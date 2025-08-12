/**
 * Удал молнией от меча
 * @type {class}
 */
module.exports = class {

  constructor(inputData){
    /** @member {number} Действие */
    this.eventId = 12;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} Урон */
    this.damage = inputData.damage;

    /** @member {number} Остаток здоровья */
    this.live = inputData.live;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
