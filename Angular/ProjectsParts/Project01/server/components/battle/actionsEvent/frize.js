/**
 * Заморозка
 * @type {Frize}
 */
module.exports = class Frize {

  constructor(inputData) {

    /** @member {number} Действие */
    this.eventId = 9;

    /** @member {string} UUID монстра */
    this.monsterUuid = inputData.monsterUuid;

    /** @member {number} Число шагов на который заморожен */
    this.steps = inputData.frizeStep;

    /** @member {number} Статус активации */
    this.isActive = inputData.isActive;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
