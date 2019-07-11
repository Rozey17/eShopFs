// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// �2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://go.microsoft.com/fwlink/p/?LinkID=258575
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

$(document).ready(function () {
  $.fn.cycle.defaults.speed = 900;
  $.fn.cycle.defaults.timeout = 6000;

  var RSec = 0;
  var RTile = 0;

  $('.tile-slide').each(function (index) {
    $(this).cycle({
      fx: 'scrollDown',
      speed: 400,
      timeout: 0
    });
  });

  AnimateTile();

  function AnimateTile() {
    RSec = Math.floor(Math.random() * 5000) + 1000;
    RTile = Math.floor(Math.random() * 5);
    setTimeout(function () {
      $('.tile-slide').eq(RTile).cycle('next');
      AnimateTile();
    }, RSec);
  }
});

$(function () {
  function getTweets() {
    var $tweets = $("#tweets");
    if ($tweets.length > 0) {
      var search = $tweets.attr("data-search");
      var url = 'http://search.twitter.com/search.json?callback=?&q=' + search;
      $.getJSON(url, function (json) {
        var output = [];
        if (json.results) {
          for (var i = 0, len = Math.min(json.results.length, 10); i < len; i++) {

            var timeDifference = (new Date().getTime() - Date.parse(json.results[i].created_at)) / (60 * 1000);
            var time;
            if (timeDifference < 60) {
              time = Math.round(timeDifference) + "m ago";
            } else {
              timeDifference = timeDifference / 60;
              if (timeDifference < 24) {
                time = Math.round(timeDifference) + "h ago";
              } else {
                time = Math.round(timeDifference / 24) + "d ago";
              }
            }
            output.push('<span class="tile__tweet" style="left: 0px; top: 0px; display: inline; position: absolute;"><span class="tile__nick"><span class="tile__time">' + time + '</span>@' + json.results[i].from_user + '</span>' + json.results[i].text + '</span>');
          }

          //now select the #results element only once and append all the output at once, then slide it into view
          $("#tweets").html(output.join(''));
          $('.tile_twitter .tile-slide').cycle({
            fx: 'scrollUp',
            speed: 400,
            timeout: 0
          });
        } else {
          setTimeout(getTweets, 2000);
        }
      });
    }
  }

  //run the getTweets function on document.ready
  getTweets();
});