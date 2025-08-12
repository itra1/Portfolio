let Helmet = require('../helmet');

/**
 * Миска
 *
 * Блокирует три удара любой силы и пропадает.
 *
 * @type {module.Bowl}
 */
module.exports = class Bowl extends Helmet{

  constructor(){
    super();

    this.uuid = '9c877b90-40f5-11e8-bcae-41e5ecbe4e9d';
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
