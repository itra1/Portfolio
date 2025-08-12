const WebSocket = require('ws');
let client = require('../components/battle/lib/client');
let notSupportPackage = require('../web-socket/packages/out/errors/notSupportedPackge');
let notValidJson = require('../web-socket/packages/out/errors/notValidJsomPackage');
let PackageLib = require('../web-socket/packagesLibrary');
let sslConfig = require('../ssl-config');

module.exports = function (app, cb) {

  process.nextTick(() => {

    const wss = new WebSocket.Server({port: 3001}, (compl) => {
      console.log('WebSocket server is started');
    });

    wss.clientList = [];
    app.wss = wss;

    wss.on('connection', function connection(ws) {

      //console.log('connection');
      ws.wss = wss;

      ws.on('message', function incoming(message) {

        console.log((new Date()).toISOString() + ' Receive: ' + message);

        let parceMessage = {};
        try {
          parceMessage = JSON.parse(message);
        } catch (e) {
          SendPackage(new notValidJson(), ws);
          app.socketValue = message;
          return;
          //ws.send((new notValidJson).GetJson());
        }

        try {
          ProcessMessage(app, ws, parceMessage);
        }catch (e) {
          app.models.DebugMailer.SendAdminMail("Server error: Ошибка ProcessMessage",e);
        }
      });

      ws.on('error', (error) => {
        console.log(error)
      });

      ws.on('close', (error) => {
        //console.log('close');
        app.battle.RemoveClient(ws);
      });

    });

    cb();

    app.wss.Send = SendPackage;

  });


};

function SendPackage(pack, ws) {
  let sendMessage = JSON.stringify(pack);

  console.log((new Date()).toISOString() + ' Send: ' + sendMessage);

  if(ws.readyState === 1)
    ws.send(sendMessage);

}

/**
 * Обработчик сообщений
 * @param app {object} Собственно само приложение
 * @param ws {object}
 * @param message
 * @constructor
 */
function ProcessMessage(app, ws, message) {

  let packet = PackageLib.GetPackage(message.packageId, message);

  // Проверка на авторизацию
  if (message.packageId > 1) {
    //todo Провека авторизации
  }

  // Проверка на не поддерживаемый пакет
  if (!packet) {
    ws.send(new notSupportPackage(), ws);
    return;
  }

  packet.Init(message.data);

  try {
    packet.Process(app, ws);
  }catch (e) {
    app.models.DebugMailer.SendAdminMail("Server error: Ошибка обработки пакета",e);
  }
}
