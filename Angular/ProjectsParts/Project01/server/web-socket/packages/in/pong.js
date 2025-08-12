let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');

module.exports = class Pong extends PackageSuper{

  constructor(){
    super();
  }

  Process(app,ws){
    ws.send('pong');
    console.log('processing Pong')
  }

};
