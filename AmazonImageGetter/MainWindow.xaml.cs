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
            resultArea.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    resultLabel.Text = "結果";
                    resultArea.Text = resultArea.Text + "\n" + text;
                })
            );
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
                if(e.Data != null)
                {
                    if (e.Data.StartsWith("result__"))
                    {
                        AddToTextArea(e.Data.Replace("result__", ""));
                    }
                    Console.WriteLine("PhantomJS output: {0}", e.Data);
                }
               
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
    var url= page.evaluate(function() {
        return document.getElementsByClassName('itemNo0')[0].getElementsByTagName('img')[0].src
    });
    console.log('result__' + url);
    phantom.exit();
});
";
            resultLabel.Text = "取得中";
            phantomJS.RunScript(scriptString, null);
        }
    }
}
