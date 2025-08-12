module.exports = function (user) {

  let SocialType = {
    EDITOR: 0,                 // Редактор
    GAME_CENTER: 1,             // IOS
    GAME_PLAY: 2,               // ANDROID
    FACEBOOK: 3,               // FACEBOOK

  };

  const EDITOR_PLAYER = 'editor';


  /**
   * Авторизация через социальную сеть
   * @param {string} token Токен социальной сети
   * @param {string} social Тип социальной сети
   * @param {Function(Error, string)} callback
   */
  user.SocialLogin = function (token, social, callback) {

    switch (social) {
      case SocialType.EDITOR:
        this.LoginEditor(token, social, callback);
        break;
      case SocialType.GAME_CENTER:
      case SocialType.GAME_PLAY:
        this.LoginLocalMobile(token, social, callback);
        break;
      case SocialType.FACEBOOK:
        callback(null, {
          result: 'NO_USE_FACEBOOK',
          token: ''
        });
        break;
    }
  };

  /**
   * Авторизация через редактор
   * @param {string} token Токен социальной сети
   * @param {string} social Тип социальной сети
   * @param {Function(Error, string)} callback
   */
  user.LoginEditor = function (token, social, callback) {

    let result = {};

    // Ищем пользователя
    user.findOne({where: {username: EDITOR_PLAYER}}, function (err, findedUser) {

      if (err != null) {
        callback(err, null);
        return;
      }

      if (!findedUser) {
        user.create({
          username: EDITOR_PLAYER,
          password: EDITOR_PLAYER,
          email: 'editor@editor.er'
        }, function (err, res) {

          if (err != null) {
            callback(err, null);
            return;
          }

          user.LoginEditor(token, social, callback);

        });
        return
      }

      user.app.models.User.login({
        username: EDITOR_PLAYER,
        password: EDITOR_PLAYER
      }, callback);

      // console.log(findedUser);
      //
      // // Далее проверяем существующий токен
      // user.app.models.AccessToken.find({where: {userId: findedUser.id}}, function (err, dataToken) {
      //
      //   // Еали не нашли токена, выполняем автризацию
      //   if (err != null || dataToken.length <= 0) {
      //
      //     user.login({username: 'editor', password: 'editor'}, callback);
      //     return;
      //   }
      //
      //   // Если всетаки нашли данные отправляем пользователю
      //   result = {
      //     result: 'OK',
      //     token: dataToken[0].id
      //   };
      //   callback(null, result);
      // })


    });
  };

  /**
   * Авторизация через социальный идентификатор
   * @param {string} token Токен социальной сети
   * @param {string} social Тип социальной сети
   * @param {Function(Error, string)} callback
   */
  user.LoginLocalMobile = function (token, social, callback) {

    if (token.length <= 0) {
      callback(null, {
        result: 'NO_TOKEN',
        token: ''
      });
      return
    }

    // Проверяем наличие ссылки на пользователя в социалках
    user.app.models.UserCredential.findOne({where: {externalId: token}}, (err, data) => {

      if (err != null || data == null) {

        // Пробуем создать ссылку
        this.CreateUserCredential(token, social, callback);
        return;
      }

      // пробуем найти актуальный логин
      user.app.models.UserCredential.findOne({where: {externalId: token}, order: 'userId ASC'}, (err, credital)=>{

        if (err != null) {
          callback(err, null);
          return;
        }

        user.app.models.User.findOne({where: {id: credital.userId}}, (err, userFinded)=>{

          if (err != null || userFinded == null) {
            callback(err, null);
            return;
          }

          user.app.models.AccessToken.find({where:{userId: userFinded.id}},(tockenErr,tokenData)=>{

            if(tockenErr){
              return user.app.models.User.login({
                username: userFinded.username,
                password: userFinded.username
              }, callback);
            }

            let token = null;

            tokenData.forEach((elem,ind,arr)=>{
              if(new Date(elem.created.setSeconds(elem.created.getSeconds() + elem.ttl)) > new Date()){
                token = elem;
              }
            });
            if(token != null){
              return callback(null,token);
            }

            return user.app.models.User.login({
              username: userFinded.username,
              password: userFinded.username
            }, callback);

          });

        });

      });

    });

  };

  /**
   * Создание пользователя
   * @param token
   * @param social
   * @param callback
   * @constructor
   */
  user.CreateUserCredential = function (token, social, callback) {

    //TODO исправить email

    user.app.models.User.create({
      username: token,
      password: token,
      email: `${token}@loopback.${social}.com`
    }, function (err, createUser) {

      if (err != null) {
        callback(err, null);
        return;
      }

      user.app.models.UserCredential.create({
        provider: social,
        authScheme: 'custom',
        externalId: token,
        userId: createUser.id
      }, function (err, userCredential) {

        //TODO проверка на ошибки

        user.app.models.User.login({
          username: token,
          password: token
        }, callback);

      });


    });

  }

  /**
   * Установка имени
   * @param {string} name Имя пользователя
   * @param {object} options Сессия
   * @param {Function(Error, object)} callback
   */

  user.setName = function (name, options, callback) {

    // options.accessToken.userId

    user.findOne({where: {and: [{name: name}, {id: {neq: options.accessToken.userId}}]}}, (err, data) => {
    //user.findOne({where: {name: name}}, (err, data) => {

      if (err)
        return callback(err, null);

      if (data != null) {
        return callback({status: 409, name: "Found", message: "Duplication name"}, null);
      }

      user.findOne({where: {id: options.accessToken.userId}}, (err, usr) => {

        if (err)
          return callback(err, null);

        usr.name = name;
        usr.save(callback);
      });

    });

  };

  /**
   * Установка значения монет
   * @param {number} coins Количество монет
   * @param {Function(Error, string)} callback
   */

  user.coinsSet = function(coins, options, callback) {

    user.findOne({where: {id: options.accessToken.userId}}, (err, data) => {
      //user.findOne({where: {name: name}}, (err, data) => {

      if (err)
        return callback(err, null);

      data.coins = coins;
      data.save((err,dat)=>{
        callback(null,data.coins);
      });
    });

  };

  /**
   * Количество монет у пользователя
   * @param {Function(Error, number)} callback
   */

  user.coinsGet = function(options, callback) {
    user.findOne({where: {id: options.accessToken.userId}}, (err, data) => {
      //user.findOne({where: {name: name}}, (err, data) => {

      if (err)
        return callback(err, null);

      callback(null,data.coins);
    });
  };

  /**
   * Получение информации о пользователе
   * @param {Function(Error, date)} callback
   */

  user.getInfo = function(options, callback) {
    user.findOne({where: {id: options.accessToken.userId}}, callback);
  };

  /**
   * Установить Email
   * @param {string} email Email пользователя
   * @param {string} metricDeviceId Идентификтор профиля
   * @param {object} options Опции
   * @param {Function(Error, string)} callback
   */

  user.setEmail = function(email, options, callback) {

    user.findOne({where:{id: options.accessToken.userId}},(err,userData) => {

      // Ошибка получения данных
      if(err != null)
        return callback({status: 501, name: "Sql error", message: "Sql error"},null);

      if(userData == null)
        return callback({status: 501, name: "No user", message: "No user"},null);

      if(userData.email === email)
        return callback({status: 501, name: "Exists email", message: "Exists email"},null);

      userData.email = email;
      userData.save();

      user.generateVerificationToken(userData,options,(errtoken,dataToken)=>{

        user.app.models.EmailVerification.destroyAll({where:{userId:userData.id}},(err,dt)=>{

          user.app.models.EmailVerification.create({
            token: dataToken,
            date: new Date(),
            userId: userData.id
          });

          let mail = 'Для подтверждения email пожалуйста перейдите по <a href="https://hbborsh.com/EmailConfirm/index.html?token='+ dataToken +'">ссылке</a>.';

          user.app.models.NorepeatMailer.sendMail(email, 'Confirm email', mail, (err,complete)=>{

          });

          callback(null, {
            result: 'ok'
          });

        });

      });

    });

  };

  /**
   * Повторная отправка мыла для подтверждения email
   * @param {object} options Опции
   * @param {Function(Error, string)} callback
   */

  user.emailConfirmRepeat = function(options, callback) {

    user.findOne({where:{id: options.accessToken.userId}},(err,userData) => {

      if(err != null)
        return callback({status: 501, name: "Sql error", message: "Duplication name"},null);

      if(userData == null)
        return callback({status: 501, name: "No user", message: "No user"},null);

      if(userData.email === null)
        return callback({status: 501, name: "No exists email", message: "No exists email"},null);

      user.app.models.EmailVerification.findOne({where:{userId:options.accessToken.userId}},(err, verifObj)=>{

        if(err != null)
          return callback({status: 501, name: "Sql error", message: "Duplication name"},null);

        if(verifObj){

          let mail = 'Для подтверждения email пожалуйста перейдите по <a href="https://hbborsh.com/EmailConfirm/index.html?token='+ verifObj.token +'">ссылке</a>.';

          user.app.models.NorepeatMailer.sendMail(userData.email, 'Confirm email', mail, (err,complete)=>{ });

          callback(null, {
            result: 'ok'
          });

        }else{
          return callback({status: 501, name: "Email confirmed", message: "Email confirmed"},null);
        }

      });

    });

  };



};
