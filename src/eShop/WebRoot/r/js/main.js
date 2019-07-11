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
    setTimeout(
      function () {
        $('.tile-slide').eq(RTile).cycle('next');
        AnimateTile();
      },
      RSec
    );
  }
});
