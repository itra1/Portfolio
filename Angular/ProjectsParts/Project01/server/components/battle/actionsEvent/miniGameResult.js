/**
 * Результат миниигры
 * @type {class}
 */
module.exports = class {

  constructor(inputData){
    /** @member {number} Действие */
    this.eventId = 21;

    this.data = inputData.data;

    console.log(this.eventId + ' ' + JSON.stringify(inputData));
  }

};
