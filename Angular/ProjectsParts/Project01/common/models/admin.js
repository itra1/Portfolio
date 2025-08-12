let uuidv1 = require('uuid/v1');
const fs = require('fs');

module.exports = function (Admin) {

  /**
   * Обновляет таблицу
   * @param model
   * @param cb
   * @method
   */
  Admin.PostgreeUpdate = function (model, cb) {

    try {
      Admin.app.dataSources.postgreesql.autoupdate(model);
    } catch (e) {
      let err = {
        error: 'Ошибка обновления модели'
      };
      cb(null, JSON.stringify(err));
      return;

    }
    cb(null, 'ok');
  };

  /**
   * Генерация
   * @param cb
   * @constructor
   */
  Admin.GenerateUuid = function (cb) {
    cb(null, uuidv1());
  };

  Admin.remoteMethod('GenerateUuid', {
    returns: {arg: 'uuid', type: 'string'},
    http: {path: '/generateUuid', verb: 'get'},
    description: 'Генерация uuid'
  });

  Admin.GetSocketValue = function (cb) {
    cb(null, Admin.app.socketValue);
  };

  Admin.remoteMethod('GetSocketValue', {
    returns: {arg: 'value', type: 'string'},
    http: {path: '/GetSocketValue', verb: 'get'},
    description: 'Тестовая функция. Значение из сокетов'
  });

  Admin.ReadPromo = function (number, callback) {

    //ReadPromoFile(3);
    ReadPromoFile(number);

    callback(null);
  };

  ReadPromoFile = function (num) {
    fs.readFile('/apps/server/promo/' + num + '.txt', "utf8", (err, data) => {
      //fs.readFile('d:/Work/BorshEvolution/server/promo/'+num+'.txt',"utf8",(err,data)=>{

      if(err)
        return;

      console.log("Start process");

      let dataArr = data.split('\n');

      for (let i = 0; i < dataArr.length; i++) {
        try {
          Admin.app.models.promo.create({

            code: dataArr[i],
            days: num

          });
        } catch (e) {
          console.log(e);
        }
      }

      console.log("Add all files");

    });
  };

  Admin.SendTestMail = function (count, callback) {

    for (let i = 0; i < count; i++)
      Admin.app.models.NorepeatMailer.sendMail('Тестовая отправка сообщений на mail' + i + '@netarchitect.ru', 'Test mass email', 'Test message ' + i + '(' + count + ')', (err, data) => {

      });
    callback(null, {
      result: 'ok'
    });
  };

  Admin.remoteMethod('SendTestMail', {
    returns: {arg: 'value', type: 'string'},
    accepts: {arg: 'count', type: 'number'},
    http: {path: '/SendTestMail', verb: 'get'},
    description: 'Тестовая отправка сообщений на mail#@netarchitect.ru'
  });

  Admin.GetBattleCount = function (callback) {

    return callback(null,
      Admin.app.battle.battles.length
    );

  };

  Admin.remoteMethod('GetBattleCount', {
    returns: {arg: 'value', type: 'string'},
    http: {path: '/GetBattleCount', verb: 'get'},
    description: 'Число активных боев'
  });

  Admin.GetSocketAuthCount = function(callback){
    return callback(null,
      Admin.app.battle.clients.length
    );
  };

  Admin.remoteMethod('GetSocketAuthCount', {
    returns: {arg: 'value', type: 'string'},
    http: {path: '/GetSocketAuthCount', verb: 'get'},
    description: 'Число авторизированных клиентов через сокеты'
  });

};
