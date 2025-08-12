let Shield = require('../shield');

/**
 * Миска
 *
 * Блокирует три удара любой силы и пропадает.
 *
 * @type {module.Bowl}
 */
module.exports = class Bowl extends Shield{

  constructor(){
    super();

    this.uuid = 'b1b7ab10-441b-11e8-bf03-2b2968267520';
    /**
     * Количество ударов, которвый плокирует
     * @type {number}
     */
    this.health = 3;
  }

  BeforeDamage(data, attack) {

    // Проверка, что удар получаем в эту часть тела
    if(!this.IsActive || !this.HitIn(data.attack))
      return;

    super.BeforeDamage(data);

    this.health--;

    // Блокирует урон
    attack.damage = 0;

  }

  AfterDamage(data, attack){

    // Проверка, что удар получаем в эту часть тела
    if (!this.IsActive || !this.HitIn(data.attack))
      return;

    if(this.health <= 0){
      super.Destroy(data,this);
    }
  }

};
