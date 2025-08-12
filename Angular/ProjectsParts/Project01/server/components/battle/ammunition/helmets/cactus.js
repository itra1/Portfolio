let Helmet = require('../helmet');
let EnumLib = require('../../../../app/enumList');
let CactusDestroyEvent = require('../../actionsEvent/cactusDestroy');

/**
 * Кактус с тремя стволами.
 *
 * Каждое попадание по нему наносит 1 урон противнику. И отваливается часть кактуса. Пропадает после трех попаданий по нему. Не блокирует урон.
 *
 * @type {Cactus}
 */
module.exports = class Cactus extends Helmet {

  constructor() {
    super();

    this.uuid = 'a51eb5e0-426a-11e8-b817-b3c2e6fc9aec';

    this.health = 3;
  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack) || attack.isBlock)
      return;

    super.BeforeDamage(data);

    this.health--;

    data.actions.push(new CactusDestroyEvent({
      monsterUuid: data.parent.monster.uuid,
      uuid: this.uuid,
      health: this.health
    }));
  }

  AfterDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack) || attack.isBlock)
      return;

    super.AfterDamage(data, attack);

    // Уничтожаем бокс
    if (this.box) {
      this.BoxDestroy(data, this);
      this.box = false;
      return;
    }

    // Нанесение урона
    let client = data.battle.clients.find(x => x.token !== data.parent.token);
    data.battle.AttackEvent(data.battle.FindActionByRound(data.battle.round),
      data.parent,
      {step: data.battle.actionNumber, action: EnumLib.actionType.headHit},
      client,
      {step: data.battle.actionNumber, action: EnumLib.actionType.none}
    );

    // Уничтожаем, если защита окончена
    if (this.health <= 0) {
      super.Destroy(data, this);
    }

  }

};
