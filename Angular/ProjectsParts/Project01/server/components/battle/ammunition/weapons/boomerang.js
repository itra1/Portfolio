let Weapon = require('../weapon');
let DamageEvent = require('../../actionsEvent/damage');
let BoomerangEvent = require('../../actionsEvent/boomerang');

/**
 * Бумеранг
 *
 * + 2 урона. Если удар бумерангом прошёл, то противник получает 3 урона, а бумеранг улетает. Если удар попал в блок,
 * то противник не получает урон, но бумеранг всё равно улетает. В любом случае, бумеранг улетает на 2 следующие хода
 * и на 3 ход по окончанию удара игрока, бумеранг возвращается и бьёт противника на 2 урона игнорируя блоки и броню.
 * Бумеранг возвращается снова в руку как оружие.
 *
 * @type {module.Boomerang}
 */
module.exports = class Boomerang extends Weapon{

  constructor(){
    super();
    this.uuid = 'c000b570-4404-11e8-a030-8708785d77e2';

    this.isOut = false;
    this.outStep = 3;

    this.isAttacked = false;
  }

  get StillReady(){
    return super.StillReady && !this.isOut && !this.isAttacked;
  }

  BeforeAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive ||!this.HitIn(data.attack) || !this.specialEffect)
      return;

    super.BeforeAttack(data,attack);

    if(!this.isOut && this.specialEffect)
      this.isAttacked = true;

    //attack.damage++;

  }

  AfterBeforeAttack(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.AfterBeforeAttack(data, attack);

    if(this.isAttacked && !attack.isBlock && attack.damage > 0 && this.specialEffect){
      attack.damage = 3;
    }

  }

  AfterAction(data){

    if(!this.IsActive || (!this.isAttacked && !this.isOut))
      return;

    super.AfterAction(data);


    if(!this.isOut){
      this.isAttacked = false;
      this.isOut = true;
      this.outStep = 3;

      data.events.push(new BoomerangEvent({
        monsterUuid: data.parent.monster.uuid,
        isOut: this.isOut
      }));

    }else{

      this.outStep--;

      if(this.outStep <= 0){
        this.isOut = false;
        this.isAttacked = false;

        data.events.push(new BoomerangEvent({
          monsterUuid: data.parent.monster.uuid,
          isOut: this.isOut
        }));

        let client = data.battle.clients.find(x => x.token !== data.parent.token);
        client.monster.SetDamage(2);

        data.events.push(new DamageEvent({
          monsterUuid: client.monster.uuid,
          damage: 2,
          live: client.monster.actualHealth
        }));

      }

    }

  }

};
