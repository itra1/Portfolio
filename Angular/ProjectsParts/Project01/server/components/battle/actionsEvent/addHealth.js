/**
 * Увеличение жизней монстра
 * @type {class}
 */
module.exports = class {

  constructor(inputData){

    /** @member {number} Действие */
    this.eventId = 7;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} Урон */
    this.medication = inputData.medication;

    /** @member {number} Остаток здоровья */
    this.live = inputData.live;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
