let PackageSuper = require('../../inPackage');

module.exports = class BattleMiniGameComplete extends PackageSuper {

  constructor(inputData) {
    super();

    /**
     * Номер события
     * @type {number}
     */
    this.step = inputData.number;
    /**
     * Время боя
     * @type {number}
     */
    this.time = inputData.time;
    /**
     * Количество очков
     * @type {number}
     */
    this.point = inputData.point;

  }

  Process(app, ws){

    let battle = app.battle.GetBattleWishClientToken(ws.token);

    if(battle != null)
      battle.PlayerMiniGameComplete(ws, this);

  }

};
