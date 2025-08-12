let Weapon = require('../weapon');
let AddPoison = require('../../actionsEvent/addPoison');
let PoisonDamageEvent = require('../../actionsEvent/poisonDamage');

/**
 * Змея
 *
 * Если удар змеёй прошёл, противник становится отравлен(сбоку горит значок), противник получает 1 урон,
 * и по одному урону следующие 2 хода. Отравления складываются. (то есть, если ударить змеёй и попасть
 * 3 раза подряд, а потом не бить, то противник получит в первый ход 1 урон, на следующий ход 2 урона,
 * на следующий 3 урона, потом 2урона и потом 1)
 *
 * @type {module.Snake}
 */
module.exports = class Snake extends Weapon {

  constructor() {
    super();
    this.uuid = 'e13ed0d0-43fc-11e8-a030-8708785d77e2';

    this.poison = [];
  }

  AfterAttack(data, attack) {

    if (this.IsActive) {
      // Проверка, что удар получаем в эту часть тела
      if (!this.HitIn(data.attack))
        return;

      super.AfterAttack(data);

      if (!attack.isBlock && attack.damage > 0 && this.specialEffect) {
        this.AddPoisonEffect(data);
      }
    }


  }

  AfterAction(data){

    this.UsePoison(data);
  }


  AddPoisonEffect(data) {

    let client = data.battle.clients.find(x => x.token !== data.parent.token);

    if (this.poison.length <= 0) {

      this.poison.push({
        client: client,
        effect: []
      });

    }
    data.actions.push(new AddPoison({
      monsterUuid: client.monster.uuid,
      step: 3,
      isActive: true
    }));

    this.poison.find(x => x.client.monster.uuid === client.monster.uuid).effect.push(3);
  }

  UsePoison(data) {

    if (this.poison.length <= 0)
      return;

    this.poison.forEach((poison, index, arr) => {

      let poisonDamage = poison.effect.length;

      for(let i = 0 ; i < poison.effect.length ; i++)
        poison.effect[i]--;

      let delInd = poison.effect.findIndex(x => x <= 0);

      while (delInd >= 0) {
        poison.effect.splice(delInd, 1);
        delInd = poison.effect.findIndex(x => x <= 0);
      }

      poison.client.monster.SetDamage(poisonDamage);

      data.events.push(new PoisonDamageEvent({
        monsterUuid: poison.client.monster.uuid,
        damage: poisonDamage,
        live: poison.client.monster.actualHealth
      }));

      if(poison.effect.length <= 0){
        data.events.push(new AddPoison({
          monsterUuid: poison.client.monster.uuid,
          step: 0,
          isActive: false
        }));
      }

    });


  }

};
