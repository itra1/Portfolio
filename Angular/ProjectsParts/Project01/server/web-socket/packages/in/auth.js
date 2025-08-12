let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');
let ClientObject = require('../../../components/battle/lib/client');
let AuthResult = require('../out/authResult');

/**
 * Авторизация
 */
module.exports = class Auth extends PackageSuper {

  constructor(inputData) {
    super();

    this.token = inputData.token; // токен авторизации
  }

  Process(app, ws) {

    //todo проверка на активный токен

    let client = new ClientObject();
    client.ws = ws;
    client.token = this.token;

    // Проверяем существование токена
    app.models.AccessToken.findOne({where: {id: this.token}}, (err, tokenModel) => {
      //todo Отправить пакет об ошибке авторизации
      if (err || tokenModel == null) {
        app.wss.Send(new AuthResult('No correct token'),ws);
        return;
      }
      ws.token = this.token;
      client.id = tokenModel.userId;
      ws.client = client;

      // Происк прользователя
      app.models.User.findOne({where: {id: tokenModel.userId}}, (err, user) => {
        if (err || user == null) {
          app.wss.Send(new AuthResult('No exists user'),ws);
          return;
        }
        client.user = user;

        app.models.MonsterLevel.find({where: {userId: tokenModel.userId}}, (err, levelData) => {

          client.levelData = levelData;
        });

        // Регистрируем игрока
        app.battle.AddClient(client);
        app.wss.Send(new AuthResult(null), ws);

      });
    });
  }

};


