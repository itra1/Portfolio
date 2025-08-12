let Helmet = require('../helmet');
let EnumLib = require('../../../../app/enumList');
let PillowFeathersEvent = require('../../actionsEvent/pillowFeathers');
let SneezeEvent = require('../../actionsEvent/sneeze');

/**
 * Подушка
 *
 * Блокирует 2 удара любой силы после чего взрывается, летают перья, игроки по очереди чихают 4 хода. Сперва
 * чихает противник(его удар или блок отменяется), а удар или блок игрока срабатывает, на следующий ход чихает
 * игрок чья была подушка, а удар или блок противника срабатывает. Дальше по-очереди.
 *
 * @type {module.GoldenCrown}
 */
module.exports = class Pillow extends Helmet {

  constructor() {
    super();

    this.uuid = '96288b40-45a5-11e8-97e3-752988b4e4e2';

    this.health = 2;

    /**
     * Чихающие клиенты
     * @type {Array}
     */
    this.clientsChih = [];

  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    attack.damage = 0;

    this.health--;

  }

  AfterDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    let client = data.battle.clients.find(x => x.token !== data.parent.token);

    // Уничтожаем, если защита окончена
    if (this.health <= 0) {

      data.actions.push(new PillowFeathersEvent({
        monsterUuid: data.parent.monster.uuid,
        uuid: this.uuid,
        isActive: true
      }));

      for (let i = 0; i < 4; i++) {
        this.clientsChih.push((i % 2 === 0 ? client : data.parent))
      }
      this.clientsChih.reverse();
      super.Destroy(data, this);
    }

  }

  BeforeAction(data) {

    if(this.clientsChih.length <= 0) return;

    let cl = this.clientsChih.pop();

    data.battle.FindActionByRound(data.battle.round).players.find(x=>x.monster === cl.monster.uuid).actions[data.battle.actionNumber].action = EnumLib.actionType.none;

    data.actions.result.find(x => x.number === data.battle.actionNumber).actions.push(new SneezeEvent({
      monsterUuid: cl.monster.uuid
    }));

    if(this.clientsChih.length <= 0){
      data.events.push(new PillowFeathersEvent({
        monsterUuid: data.parent.monster.uuid,
        uuid: this.uuid,
        isActive: false
      }));
    }

  }


};
