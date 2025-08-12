'use strict';

module.exports = function(EmailVerification) {

  const CONFIRM_STATUS_NO_CONFIRM = 'NOCONFIRM';
  const CONFIRM_STATUS_CONFIRM = 'CONFIRM';
  const CONFIRM_STATUS_REPEAT = 'REPEAT';

  /**
   * Подтверждение email
   * @param {string} token Токен подтверждения
   * @param {Function(Error, string)} callback
   */

  EmailVerification.confirmEmail = function(token, callback) {

    EmailVerification.findOne({where: {token:token}},(err,data)=>{

      if(err != null || data == null || data.isConfirm === true)
        return callback(null,{
          result: "NO_EXISTS"
        });

      data.isConfirm = true;
      data.save();

      return callback(null,{
        result: "CONFIRM"
      });

    });

  };

  /**
   * Проверка подтверждения почты
   * @param {Function(Error, string)} callback
   */
  EmailVerification.checkEmailConfirm = function(options, callback) {

    EmailVerification.findOne({where: {userId:options.accessToken.userId}},(err,data)=>{

      if(err != null || data == null)
        return callback(null,{
          result: CONFIRM_STATUS_REPEAT
        });

      if(data.isConfirm !== true)
        return callback(null,{
          result: CONFIRM_STATUS_NO_CONFIRM
        });

      data.destroy();

      return callback(null,{
        result: CONFIRM_STATUS_CONFIRM
      });

    });

  };


};
