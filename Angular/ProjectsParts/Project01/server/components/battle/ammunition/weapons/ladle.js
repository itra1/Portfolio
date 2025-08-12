let Weapon = require('../weapon');
let EnumLib = require('../../../../app/enumList');
let StunEvent = require('../../actionsEvent/stun');

/**
 * Поварешка
 *
 * Поварешка дает +1 урона. Если у противника нет шлема, и он пропускает удар в голову, то поварёшка наносит 2 урона и пропадает, а противник оглушается на 3 хода (не защищается и не нападает).
 *
 * @type {module.Brick}
 */
module.exports = class Ladle extends Weapon {

  constructor() {
    super();

    this.uuid = 'ded8ada0-43f9-11e8-a030-8708785d77e2';

    this.inHead = false;

    this.stunCount = 3;

  }

  BeforeAction(data) {

    super.BeforeAction(data);

    if (this.inHead && !this.IsActive && this.stunCount > 0) {

      this.stunCount--;

      let client = data.battle.clients.find(x => x.token !== data.parent.token);
      data.actions.players.find(x => x.monster === client.monster.uuid).actions[data.battle.actionNumber].action = EnumLib.actionType.none;

      if(this.stunCount <= 0){
        data.events.push(new StunEvent({
          step: 0,
          monsterUuid: client.monster.uuid,
          isActive: false
        }));
      }

    }

  }

  BeforeAttack(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack) || !this.specialEffect)
      return data.damage;

    super.BeforeAttack(data, attack);

    attack.damage++;

  }

  AfterBeforeAttack(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack) || !this.specialEffect)
      return;

    super.AfterBeforeAttack(data, attack);

    if (!attack.isBlock && attack.damage >=2) {

      let client = data.battle.clients.find(x => x.token !== data.parent.token);

      let helm = client.ammunitions.find(x=>x.ammunition === 1 && x.IsActive);

      if(helm == null){
        if (data.attack === EnumLib.actionType.headHit) {
          attack.damage = 2;

          this.inHead = true;

        }
      }
    }
  }

  AfterAttack(data) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.AfterAttack(data);

    let client = data.battle.clients.find(x => x.token !== data.parent.token);

    if (this.inHead) {

      data.actions.push(new StunEvent({
        step: this.stunCount,
        monsterUuid: client.monster.uuid,
        isActive: true
      }));

      super.Destroy(data, this);

    }

  }

};
