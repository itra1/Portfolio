let Shield = require('../shield');
let FrizeEvent = require('../../actionsEvent/frize');
let EnumLib = require('../../../../app/enumList');

/**
 * Кусок льда
 *
 * При попадании по кусочку льда, он блокирует 1 урон (остальной урон проходит), замораживает противника на 1 ход  и пропадает.
 *
 * @type {module.Ice}
 */
module.exports = class Ice extends Shield {

  constructor() {
    super();
    this.uuid = '0ff2e5e0-44e0-11e8-ab11-eb0b40b7e297';

    this.frizeClient = null;

  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    attack.damage--;

  }

  AfterDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    let client = data.battle.clients.find(x => x.token !== data.parent.token);

    data.actions.push(new FrizeEvent({
      monsterUuid: client.monster.uuid,
      frizeStep: 1,
      isActive: true
    }));

    this.frizeClient = client;

    super.Destroy(data, this);
  }

  BeforeAction(data){

      if (this.frizeClient == null) return;

      super.BeforeAction(data);
      data.battle.FindActionByRound(data.battle.round).players.find(x => x.monster === this.frizeClient.monster.uuid).actions[data.battle.actionNumber].action = EnumLib.actionType.none;


      data.events.push(new FrizeEvent({
        monsterUuid: this.frizeClient.monster.uuid,
        frizeStep: 0,
        isActive: false
      }));
      this.frizeClient = null;

  }
};

