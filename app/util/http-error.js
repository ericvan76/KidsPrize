'use strict';

/**
 * @constructs HttpError
 * @param {Number} status  [description]
 * @param {String} [message] [description]
 */
function HttpError(status, message) {
  Error.apply(this, arguments);
  this.status = status;
  this.message = message;
}

module.exports = HttpError;
