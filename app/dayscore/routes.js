'use strict';

var DayScore = require('./dayscore'),
  crud = require('../crud');

var dayScoreCrud = crud(DayScore, {
  userRestrict: true,
  path: '/child/:cid/dayscore',
  include: ['create', 'read', 'patch', 'delete', 'query']
});

dayScoreCrud.router.patch('/dayscore/:id', function(req, res, next) {

});

module.exports = dayScoreCrud.router;

// var Dayscore = require('./dayscore'),
//   crud = require('../crud');

// var controller = crud.controller(Dayscore, {
//   owerRestrict: true
// });


// var routes = crud.router

// module.export = controller;

// DayScoreSchema.statics.setScore = function(childId, date, task, score, callback) {
//   exports.findOneAndUpdate({
//       _child: childId,
//       date: date
//     }, {
//       $setOnInsert: {
//         date: date
//       }
//     }, {
//       new: true,
//       upsert: true
//     },
//     function(err, dayScore) {
//       if (err) {
//         callback(err);
//       } else {
//         try {
//           // find existing task entry
//           var existing = dayScore.scores.filter(function(s) {
//             return s.task.toLowerCase() === task.toLowerCase();
//           }).pop();

//           if (existing) {
//             if (existing.score !== score) {
//               // update task's score
//               mongoose.model('DayScore').update({
//                 _id: dayScore._id,
//                 __v: dayScore.__v,
//                 'scores.task': task
//               }, {
//                 $set: {
//                   'scores.$.score': score,
//                   'scores.$.update_at': new Date(),
//                   update_at: new Date()
//                 },
//                 $inc: {
//                   __v: 1
//                 }
//               }, function(err, n) {
//                 if (err) {
//                   callback(err);
//                 } else {
//                   mongoose.model('DayScore').findById(dayScore._id, callback);
//                 }
//               });
//             } else {
//               callback(null, dayScore);
//             }
//           } else {
//             // insert task's 1st score
//             mongoose.model('DayScore').update({
//               _id: dayScore._id,
//               __v: dayScore.__v
//             }, {
//               $addToSet: {
//                 scores: {
//                   task: task,
//                   score: score
//                 }
//               },
//               $set: {
//                 update_at: new Date()
//               },
//               $inc: {
//                 __v: 1
//               }
//             }, function(err, n) {
//               if (err) {
//                 callback(err);
//               } else {
//                 mongoose.model('DayScore').findById(dayScore._id, callback);
//               }
//             });
//           }
//         } catch (e) {
//           callback(e);
//         }
//       }
//     }
//   );
// };
