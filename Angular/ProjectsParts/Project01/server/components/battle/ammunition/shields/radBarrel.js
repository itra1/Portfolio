let Shield = require('../shield');
let RadDamageEvent = require('../../actionsEvent/radDamage');

/**
 * Бочка со значком радиации.
 *
 * Бочка блокирует 3 удара,  после чего взрывается, над игроками появляются значки радиации и они оба получают по
 * случайному урону(1 или 2) каждый ход, до конца боя.
 *
 * @type {module.RadBarrel}
 */
module.exports = class RadBarrel extends Shield{

  constructor(){
    super();

    this.uuid = '0c1771a0-4339-11e8-ada2-7b67d221757b';

    this.health = 3;

    this.box = true;
  }

  BeforeDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    if(this.box)
      return;

    super.BeforeDamage(data);

    attack.damage = 0;
    this.health--;

  }

  AfterDamage(data, attack){

    if(!this.IsActive){

      let damage = Math.round(Math.random())+1;

      let client = data.battle.clients.find(x => x.token !== data.parent.token);

      client.monster.SetDamage(damage);
      data.actions.push(new RadDamageEvent({
        monsterUuid: client.monster.uuid,
        damage: damage,
        live: client.monster.actualHealth
      }));

      damage = Math.round(Math.random())+1;
      data.parent.monster.SetDamage(damage);
      data.actions.push(new RadDamageEvent({
        monsterUuid: data.parent.monster.uuid,
        damage: damage,
        live: data.parent.monster.actualHealth
      }));
    }

    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    // Уничтожаем бокс
    if(this.box){
      this.BoxDestroy(data,this);
      this.box = false;
      return;
    }

    // Уничтожаем, если защита окончена
    if(this.health <= 0){
      super.Destroy(data,this);
    }

  }

};
