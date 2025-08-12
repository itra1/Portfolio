let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');
let OutPongPackage = require('../out/pong');

module.exports = class Ping extends PackageSuper{

  constructor(){
    super();
  }

  Process(app,ws){
    app.wss.Send(new OutPongPackage(),ws);
  }

};
