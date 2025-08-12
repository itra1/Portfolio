/**
 * Класс отвечающий за монстра
 * @type {module.Monster}
 */
module.exports = class Monster {

  constructor(monster,params) {

    this.uuid = monster.uuid;

    /**
     * Максимальное значение жизней
     * @type {number}
     */
    this.maxHealth = params.health;
    /**
     * Актуальное значение жизней
     * @type {number}
     */
    this.actualHealth = params.health;

    this.monster = monster;

  }

  /**
   * Инициализация боя
   * @method
   */
  BattleInitiate(){
    this.actualHealth = this.maxHealth;
  }

  /**
   * Он мертв
   * @return {boolean} IsDead Он мертв
   * @property
   */
  get IsDead() {
    return this.actualHealth <= 0;
  }

  /**
   * Нанесение урона монстру
   * @param value {number} Размен нанесенного урона
   * @method
   */
  SetDamage(value) {
    this.actualHealth -= value;
  }

  AddHealth(value){
    this.actualHealth += value;
  }

};
