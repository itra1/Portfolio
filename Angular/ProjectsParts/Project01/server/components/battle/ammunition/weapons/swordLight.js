let Weapon = require('../weapon');
let SwordLightEvent = require('../../actionsEvent/stormLight');

/**
 * Меч с молниями
 *
 * +1 к урону. Каждый второй ход происходит один случайный удар молнии, он может попасть как в противника,
 * так и во владельца меча, и наносит 1 урон.
 *
 * @type {module.SwordLight}
 */
module.exports = class SwordLight extends Weapon {

  constructor() {
    super();

    this.uuid = 'c9d32d00-4407-11e8-a030-8708785d77e2';

    this.step = 0;

  }

  BeforeAttack(data, attack) {

    if (!this.IsActive || !this.HitIn(data.attack) && !this.specialEffect)
      return;

    super.BeforeAttack(data);

    attack.damage++;

  }

  AfterAction(data){
    if (!this.specialEffect)
      return;

    this.step++;

    let use = ((this.step & 1) === 0);

    if(use){

      let client = data.battle.clients[Math.round(Math.random())];

      client.monster.SetDamage(1);

      data.events.push(new SwordLightEvent({
        monsterUuid: client.monster.uuid,
        damage: 1,
        live: client.monster.actualHealth
      }));
    }

  }

};
