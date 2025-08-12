let Helmet = require('../helmet');
let AddHealth = require('../../actionsEvent/addHealth');

/**
 * Графин с зельем
 *
 * После 3х ударов, он раскалывается, пропадает и лечит питомца на 2 жизни. Не блокирует урон.
 *
 * @type {Potion}
 */
module.exports = class Potion extends Helmet{

  constructor(){
    super();

    this.uuid = '6d6ef1c0-3e85-11e8-b28a-ff0bd550df3f';

    /**
     * Количество жизней, на сколько вылечить
     * @type {number}
     */
    this.addHealth = 2;

    /**
     * Количество ударов до разбития
     * @type {number}
     */
    this.health = 3;

    this.box = true;
  }

  BeforeDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    if(this.box)
      return;

    this.health--;

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

    // Уничтожаем, если защита окончена
    if(this.health <= 0){
      data.parent.monster.AddHealth(this.addHealth);
      data.actions.push(new AddHealth({
        monsterUuid: data.parent.monster.uuid,
        medication: this.addHealth,
        live: data.parent.monster.actualHealth
      }));

      super.Destroy(data,this);
    }

  }

};
