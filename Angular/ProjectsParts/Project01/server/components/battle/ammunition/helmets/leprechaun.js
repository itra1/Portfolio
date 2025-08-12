let Helmet = require('../helmet');
let InverseActionsEvent = require('../../actionsEvent/inverseActions');
let EnumLib = require('../../../../app/enumList');

/**
 * Ирландский лепрекон
 * «появляется в ящике»
 *
 * Все блоки противника меняются на соответствующие удары, а удары на блоки.
 * Пропадает после трех пропущенных ударов в голову.
 *
 * @type {Leprechaun}
 */
module.exports = class Leprechaun extends Helmet {

  constructor() {
    super();

    this.uuid = 'fbd5fb30-3e84-11e8-b28a-ff0bd550df3f';

    /**
     * Максимальное чило полученнударов, которое выдержит
     * @type {number}
     */
    this.protectHit = 3;
    /**
     * Стартует в ящике
     * @type {boolean}
     */
    this.box = true;

    this.isInverce = false;
  }

  BeforeAction(data) {

    if (!this.IsActive)
      return;

    super.BeforeAction(data);

    if (!this.box) {

      let client = data.battle.clients.find(x => x.token !== data.parent.token);

      let action = data.actions.players.find(x => x.monster === client.monster.uuid);
      console.log("Start " + action.actions[data.battle.actionNumber].action);

      switch (action.actions[data.battle.actionNumber].action) {
        case EnumLib.actionType.bodyBlock:
          action.actions[data.battle.actionNumber].action = EnumLib.actionType.bodyHit;
          break;
        case EnumLib.actionType.bodyHit:
          action.actions[data.battle.actionNumber].action = EnumLib.actionType.bodyBlock;
          break;
        case EnumLib.actionType.headHit:
          action.actions[data.battle.actionNumber].action = EnumLib.actionType.headBlock;
          break;
        case EnumLib.actionType.headBlock:
          action.actions[data.battle.actionNumber].action = EnumLib.actionType.headHit;
          break;
      }

    }


  }


  // BeforeRound(data, attack){
  //
  //   // Проверка, что удар получаем в эту часть тела
  //   if (!this.IsActive || !this.HitIn(data.attack))
  //     return;
  //
  //   super.BeforeRound(data);
  //
  //   let client = data.battle.clients.find(x=>x.token !== data.parent.token);
  //
  //   data.actions.push(new InverseActionsEvent({
  //     monsterUuid: client.monster.uuid
  //   }));
  //
  //   let actions = data.actions.players.find(x=>x.monster === client.monster.uuid);
  //
  //   actions.forEach(elem =>{
  //
  //     if(elem.action === EnumLib.actionType.bodyBlock)
  //       elem.action = EnumLib.actionType.bodyHit;
  //     if(elem.action === EnumLib.actionType.bodyHit)
  //       elem.action = EnumLib.actionType.bodyBlock;
  //     if(elem.action === EnumLib.actionType.headBlock)
  //       elem.action = EnumLib.actionType.headHit;
  //     if(elem.action === EnumLib.actionType.headHit)
  //       elem.action = EnumLib.actionType.headBlock;
  //   });
  //
  // }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    if (this.box)
      return;

    this.protectHit--;

  }


  AfterDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    // Уничтожаем бокс
    if (this.box) {
      this.BoxDestroy(data, this);
      this.box = false;
      return;
    }

    // Уничтожаем, если защита окончена
    if (this.protectHit <= 0) {
      super.Destroy(data, this);
    }
  }

};
