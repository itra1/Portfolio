let Weapon = require('../weapon');
let AddHealthEvent = require('../../actionsEvent/addHealth');

/**
 * Мечь вампира
 *
 * +1 к урону, каждый второй удар, который прошел, добавляет +1 к жизни владельцу меча.
 *
 * @type {module.SwordVampire}
 */
module.exports = class SwordVampire extends Weapon {

  constructor() {
    super();

    this.uuid = 'a4b69a90-43f1-11e8-a030-8708785d77e2';

    this.attackCount = 0;
  }

  BeforeAttack(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack))
      return;

    attack.damage++;

  }

  AfterAttack(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.HitIn(data.attack) || !this.specialEffect)
      return;

    super.AfterAttack(data, attack);

    let isAttacked = false;
    if (!attack.isBlock && attack.damage > 0) {
      this.attackCount++;
      isAttacked = true;
    }

    if (isAttacked) {

      isAttacked = false;

      if ((this.attackCount & 1) === 0) {

        data.parent.monster.AddHealth(1);

        data.actions.push(new AddHealthEvent({
          monsterUuid: data.parent.monster.uuid,
          medication: 1,
          live: data.parent.monster.actualHealth
        }));
      }
    }

  }

};
