let uuidv1 = require('uuid/v1');

module.exports = function (Monster, ctx) {

  /**
   * Создание UUID при сохранении
   */
  Monster.observe('before save', function CreateUUD(ctx, next) {
    if (ctx.instance) {
      if(ctx.instance.uuid === "" || ctx.instance.uuid===null || ctx.instance.uuid==="string" || ctx.instance.uuid===undefined)
        ctx.instance.uuid = uuidv1();
    }
    next();
  });

  /**
   * Создание монстра
   *
   * @param {object} options Информация о сессии
   * @param {callback} callback
   */
  Monster.CreateMonster = function (targetLevel, options, callback) {

    if (options.accessToken == null) {
      return callback(null, null);
    }

    let level = 15;

    if(targetLevel != null)
      level = targetLevel;

    if(level == null)
      level = 15;

    this.CreateByLevel(options.accessToken.userId, level, callback);

  };

  Monster.CreateByLevel = function (userId, level, callback) {
    Monster.create(
      {
        level: level,
        userId: userId
      }, callback);
  };

  /**
   * Объединение монстров
   * @param {string} firstMonsterUuid Первый монстр Uuid
   * @param {string} secondMonsterIdUuid Второй монстр Uuid
   * @param {Function(Error, object)} callback
   */

  Monster.MonsterMerge = function (firstMonsterUuid, secondMonsterIdUuid, callback) {

    Monster.find({where: {or: [{uuid: firstMonsterUuid}, {uuid: secondMonsterIdUuid}]}}, (err, monsterList) => {

      if (err != null) {
        return callback(err, null);
      }

      if (monsterList.length < 2) {
        return callback({
          status: 400,
          message: 'No correct uuid'
        }, null);
      }

      if (monsterList[0].userId !== monsterList[1].userId) {
        return callback({
          status: 400,
          message: 'Monsters belong to different users'
        }, null);
      }

      if (monsterList[0].level !== monsterList[1].level) {
        return callback({
          status: 400,
          message: 'The levels are different'
        }, null);
      }

      Monster.app.models.MonsterParam.find({where: {level: monsterList[0].level}}, (err, param) => {

          if (err != null)
            return callback(err, null);

          if (monsterList[0].wins < param.needWin || monsterList[1].wins < param.needWin) {

            return callback({
              status: 400,
              message: 'Недостаточное количество побев у одного из родителей'
            }, null);
          }

          monsterList[0].isRemoved = true;
          monsterList[0].save();
          monsterList[1].isRemoved = true;
          monsterList[1].save();

          this.CreateByLevel(monsterList[0].userId, monsterList[0].level + 1, function (err, data) {

            if (err) {
              callback(err, null);
              return;
            }

            callback(null, data);

          });

        }
      );

    });

  };

  Monster.UpdateDescription = function (uuid, name, description, options, callback) {


    Monster.findOne({where: { and: [{uuid: uuid},{userId: options.accessToken.userId}]}},(err,monst)=>{

      if(err){
        return callback(err,null);
      }

      if(monst == null){
        let err1 = new Error('Не найдет монстр');
        err1.statusCode = 204;
        err1.code = 'NOT_FOUND';
        return callback(err1);
      }

      monst.name = name;
      monst.description = description;
      return monst.save(callback);
    });

  };


  /**
   * Описание метода
   * @param {string} monsterUuid Uuid монстра
   * @param {object} options Параметры сессии
   * @param {Function(Error)} callback
   */
  Monster.Delete = function (monsterUuid, options, callback) {

    Monster.findOne({where: {and: [{uuid: monsterUuid}, {userId: options.accessToken.userId}]}}, (err, monst) => {
      if (err)
        return callback(err, null);

      if(monst == null){
        let err1 = new Error('Не найдет монстр');
        err1.statusCode = 204;
        err1.code = 'NOT_FOUND';
        return callback(err1);
      }


      monst.isRemoved = true;
      monst.save(null,(err,res)=>{
        if (err)
          return callback(err, null);

        return callback(null, "ok");

      });
      //return callback(null, true);
    });
  };

  /**
   * Список своих монастров
   * @param {object} options Параметры сессии
   * @param {Function(Error, array)} callback
   */

  Monster.getList = function (options, callback) {

    Monster.find({where: {and: [{userId: options.accessToken.userId},{isRemoved: false}]}}, (err, monserList) => {

      if (err != null) {
        return callback(err, null);
      }

      Monster.app.models.MonsterLevel.find({where: {userId: options.accessToken.userId}}, (err, colorList) => {
          if (err != null)
            return callback(err, null);

          return callback(null, {
            monsters: monserList,
            levels: colorList
          });

        }
      )

    });
  };

  Monster.afterRemote('getList', function(context, remoteMethodOutput, next) {

    let monsterLen = context.result.monsters.length;

    if(monsterLen === 0)
      return next();

    context.result.monsters.forEach((elem,ind,err)=>{

      elem.parents(null,(err,res)=>{

        elem.parents = res;

        monsterLen--;

        if(monsterLen <= 0)
          return next();

      });
    });
  });

  /**
   * Изменение очков побед
   * @param {string} monsterUuid UUID монстра
   * @param {number} point Число очков, на которое необходимо изменить
   * @param {Function(Error, )} callback
   */
  Monster.changeWinPoint = function(monsterUuid, point, callback) {

    Monster.findOne({where: {and: [ {uuid: monsterUuid}, {isRemoved: false}]}},(err,mstr)=>{

      if(err)
        return callback(err);

      if(mstr == null){
        let err1 = new Error('Не найдет монстр');
        err1.statusCode = 204;
        err1.code = 'NOT_FOUND';
        return callback(err1);
      }

      mstr.wins += point;
      mstr.save(callback);

    });

  };


};
