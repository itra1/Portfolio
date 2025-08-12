let webSocket = require('../../../boot/webSocket');
let PackageSuper = require('../../inPackage');
let BattleWait = require('../out/battleWait');
let MonsterData = require('../../../components/battle/lib/monster');
let AmmunitionLibrary = require('./../../../components/battle/ammunition/ammunitionLibrary');

/**
 * Запрос на запуск боя
 * @type {class}
 */
module.exports = class BattleRequest extends PackageSuper {

  constructor(inputData) {
    super();

    /**
     * Uuid монстра запрашиваемый в бой
     * @type {string}
     */
    this.monsterUuid = inputData.monsterUuid;

    /**
     * Необходим ли бот
     * @type {boolean}
     */
    this.boat = inputData.boat;

    this.timeOutBoat = inputData.timeOutBoat;

    /**
     * Массив амуниции
     * @type {*|Array}
     */
    this.ammun = inputData.ammunitions;
    this.ammunitions = [];
  }

  Process(app, ws) {

    console.log('parce start');

    // Ищем монстра
    app.models.Monster.findOne({where: {uuid: this.monsterUuid}}, (err, monster) => {

      if (err || monster == null) {
        //todo реализовать обработчик отсутствия монстра
        return;
      }

      // Ищем клиента
      let client = app.battle.GetClientByToken(ws.token);

      if (client == null) return;

      client.monster = new MonsterData(monster, app.battle.monsterParams.find(x => x.level === monster.level));


      this.ParsingAmmunitions();

      client.ammunitions = this.ammunitions;

      let opponent = null;

      if (!this.boat) {
        // Ищем соперника соответствующего вида
        opponent = app.battle.FindOpponent(client.monster);

        if (!opponent) {
          app.battle.AddReadyClient(client,this.timeOutBoat);
          return app.wss.Send(new BattleWait(), ws);
        }
        app.battle.CreateBattle(client, opponent);

      } else {
        // Если требуется бот, создаем его

        app.battle.StartBattleWishBoat(client);
      }

    });

  }

  ParsingAmmunitions() {

    for (let i = 0; i < this.ammun.length; i++) {
      if(this.ammun[i] != null)
        this.ammunitions.push(AmmunitionLibrary.GetAmmunition(this.ammun[i].uuid))
    }

  }

};
