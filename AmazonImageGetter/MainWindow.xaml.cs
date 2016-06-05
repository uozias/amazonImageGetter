using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NReco.PhantomJS;
using System.Collections;

namespace AmazonImageGetter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // イベント設定
            SetEvents();
        }

        /// <summary>
        /// イベント設定
        /// </summary>
        public void SetEvents()
        {
            goButton.Click += OnCopyButtonClick;
        }

        void OnCopyButtonClick(object sender, RoutedEventArgs e)
        {
            String text = asinInput.Text;
            String url = GetAmazonUrl(text);
            GetAmazonImageUrls(url);
        }

        void AddToTextArea(String text)
        {
            resultArea.Text = resultArea.Text + "\n" + text;
        }

        String GetAmazonUrl(String asin)
        {
            String result = "http://www.amazon.co.jp/x/dp/REPELACE_ME_BY_ASIN?ie=UTF8&redirect=true";
            return result.Replace("REPELACE_ME_BY_ASIN", asin); ;
        }

        void GetAmazonImageUrls(String url)
        {
            var result = new ArrayList();
            var phantomJS = new PhantomJS();
            phantomJS.OutputReceived += (sender, e) => {
                AddToTextArea(e.Data);
                Console.WriteLine("PhantomJS error: {0}", e.Data);
            };
            phantomJS.ErrorReceived += (sender, e) => {
                Console.WriteLine("PhantomJS error: {0}", e.Data);
            };

            var scriptString = @"
var webPage = require('webpage');
var system = require('system');
var args = system.args;

var page = webPage.create();

page.onConsoleMessage = function(msg) {
  console.log(msg);
}

page.open('"+ url + @"', function(status) {
  page.evaluate(function() {
    var lists = document.getElementById('altImages').children[1].children;
    for(var j = 1; j < lists.length + 1; j++){
        lists[j].click();
        var result = document.getElementsByClassName('itemNo' + j)[0].getElementsByTagName('img')[0].src
        console.log(result);
    }
    phantom.exit();
  });
});
";
            phantomJS.RunScript(scriptString, null);
        }
    }
}
