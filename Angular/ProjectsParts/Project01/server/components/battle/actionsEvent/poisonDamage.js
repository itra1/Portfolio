/**
 * Получение урона от отравления
 * @type {class}
 */
module.exports = class {

  constructor(inputData){
    /** @member {number} Действие */
    this.eventId = 16;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} Урон */
    this.damage = inputData.damage;

    /** @member {number} Остаток здоровья */
    this.live = inputData.live;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
