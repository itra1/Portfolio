let PongInPackage = require('./packages/in/pong');
let PingInPackage = require('./packages/in/ping');
let AuthPackage = require('./packages/in/auth');
let BattleRequest = require('./packages/in/battleRequest');
let BattleComplete = require('./packages/in/roundComplete');
let BattleActions = require('./packages/in/roundAction');
let BattleActionsComplete = require('./packages/in/battleActionComplete');
let BattleMiniGameComplete = require('./packages/in/battleMiniGameComplete');

module.exports = class SocketList {

  static GetPackage(packageId, message) {

    switch (packageId) {
      case 1: return new PingInPackage(message);
      case 2: return new PongInPackage(message);
      case 3: return new AuthPackage(message);
      case 4: return new BattleRequest(message);
      case 5: return new BattleActions(message);
      case 6: return new BattleActionsComplete(message);
      case 7: return new BattleMiniGameComplete(message);
    }

    console.error('WS: Не определен пакет с идентификатором %s', packageId);

    return null;
  }

};
