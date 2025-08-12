'use strict';

module.exports = function(Promo) {

  /**
   * Промо код
   * @param {number} days Количество дней
   * @param {object} options Опции сессии
   * @param {Function(Error, object)} callback
   */

  Promo.GetCode = function(days, options, callback) {

    if (options.accessToken == null)
      return callback(null, null);

    if(days !== 7 && days !== 3)
      return callback({
        status: 100,
        message: 'No correct day count'
      }, null);

    Promo.findOne({where:{and: [{userId: options.accessToken.userId},{days: days},{issued:{ gt: (new Date(new Date())-(days*86400000))}}]}},(err,data)=>{

      if (err != null)
        return callback(err, null);

      if(data != null)
        return callback(null, data);

      Promo.findOne({where:{and: [{days: days},{userId: null}]}},(err,data)=>{

        if (err != null)
          return callback(err, null);

        if(data == null)
          return callback({
            status: 100,
            message: 'No new code'
          }, null);

        data.issued = new Date();
        data.userId = options.accessToken.userId;

        data.save(callback);
      });

    });

    // Promo.find({where:{and: [{userid: options.accessToken.userId},{days: days},{issued: (new Date())+days}]}},(err,data)=>{
    //
    //   console.log(err);
    //   console.log(data);
    //
    // });

  };


};
