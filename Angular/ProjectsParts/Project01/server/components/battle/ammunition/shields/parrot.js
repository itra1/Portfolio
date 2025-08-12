let Shield = require('../shield');
let StealAmmunitionEvent = require('../../actionsEvent/stealAmmunition');

/**
 * Попугай в клетке
 *
 * Блокирует два удара любой силы, после чего клетка ломается, попугай вылетает, забирает рендомный предмет у противника и улетает с ним. Если ничего нет, то просто улетает.
 *
 * @type {module.GoldenCrown}
 */
module.exports = class Parrot extends Shield{

  constructor(){
    super();

    this.uuid = '5632ad30-4df8-11e8-9b88-29bace3cb1ad';

    this.health = 2;

    this.box = true;

  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data, attack);

    if(this.box)
      return;

    this.health--;

    // Блокирует урон
    attack.damage = 0;

  }

  AfterDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    super.AfterDamage(data, attack);

    // Уничтожаем бокс
    if(this.box){
      this.BoxDestroy(data,this);
      this.box = false;
      return;
    }

    if (this.health <= 0) {
      let client = data.battle.clients.find(x => x.token !== data.parent.token);
      let activeAmmunit = client.ammunitions.filter(x=>x.StillReady);

      // кража
      if (activeAmmunit.length > 0) {
        let ind = Math.floor(Math.random() * (activeAmmunit.length));
        let amm = client.ammunitions[ind];


        console.log("Попугай кража");
        console.log(ind);
        console.log(amm);

        if(amm != null && amm != undefined) {
          data.actions.push(new StealAmmunitionEvent({
            monsterUuid: client.monster.uuid,
            ammunitionUuid: amm.uuid
          }));
          client.ammunitions.splice(ind, 1);
        }
      }

      super.Destroy(data, this);
    }

  }

};
