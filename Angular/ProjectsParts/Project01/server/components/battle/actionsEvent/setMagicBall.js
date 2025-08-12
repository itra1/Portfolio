/**
 * Установка значения магического шара
 * @type {class}
 */
module.exports = class {

  constructor(inputData){
    /** @member {number} Действие */
    this.eventId = 20;

    this.monsterUuid = inputData.monsterUuid;

    this.value = inputData.value;

    this.uuid = inputData.uuid;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
