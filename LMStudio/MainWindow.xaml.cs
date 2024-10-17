using LMStudio.Acceso_a_datos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Media.Animation;

namespace LMStudio
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string modelLlama = "QuantFactory/Meta-Llama-3-8B-Instruct-GGUF";
        private static string model2 = "TheBloke/dolphin-2.2.1-mistral-7B-GGUF";
        private bool stop = false;
        public MessageCollection messages = new MessageCollection();
        private Storyboard scrollViewerStoryboard;
        private DoubleAnimation scrollViewerScrollToEndAnim;
        private IModel channel;


        private MessageSide curside;

        #region VerticalOffset DP

        /// <summary>
        /// VerticalOffset, a private DP used to animate the scrollviewer
        /// </summary>
        private DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset",
          typeof(double), typeof(MainWindow), new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MainWindow app = d as MainWindow;
            app.OnVerticalOffsetChanged(e);
        }

        private void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            ConversationScrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            string initialMessage = "Hola";

            messages.Add(new Message()
            {
                Side = MessageSide.You,
                Text = initialMessage
            });

            curside = MessageSide.You;

            this.DataContext = messages;

            scrollViewerScrollToEndAnim = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new SineEase()
            };
            Storyboard.SetTarget(scrollViewerScrollToEndAnim, this);
            Storyboard.SetTargetProperty(scrollViewerScrollToEndAnim, new PropertyPath(VerticalOffsetProperty));

            scrollViewerStoryboard = new Storyboard();
            scrollViewerStoryboard.Children.Add(scrollViewerScrollToEndAnim);
            this.Resources.Add("foo", scrollViewerStoryboard);

        }

        private void ScrollConversationToEnd()
        {
            scrollViewerScrollToEndAnim.From = ConversationScrollViewer.VerticalOffset;
            scrollViewerScrollToEndAnim.To = ConversationContentContainer.ActualHeight;
            scrollViewerStoryboard.Begin();
        }
        private void addTextMe(string text)
        {
            messages.Add(new Message()
            {
                Side = MessageSide.Me,
                Text = text,
                PrevSide = curside
            });

            curside = MessageSide.Me;

            ScrollConversationToEnd();
        }

        private void addTextYou(string text)
        {
            messages.Add(new Message()
            {
                Side = MessageSide.You,
                Text = text,
                PrevSide = curside
            });

            curside = MessageSide.You;

            ScrollConversationToEnd();


        }
        private async void RabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "messages",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
            arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnReceived;
            channel.BasicConsume(queue: "messages",
                                 autoAck: true,
                                 consumer: consumer);
            string message = "Hola";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "messages", basicProperties: null, body: body);

        }
        private async void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
            // Actualiza la UI de manera segura usando Dispatcher
            Dispatcher.Invoke(new Action(() =>
            {
                addTextYou(message);
            }));
            string response = await LMApi.PostAPIAsync(message, modelLlama);
            Dispatcher.Invoke(new Action(() =>
            {
                addTextMe(response);
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            channel?.Close();
            channel?.Dispose();
            base.OnClosed(e);
        }
        private async void btn_start_Click(object sender, RoutedEventArgs e)
        {
            
            string message = "Hola";
            while (!stop)
            {
                string response = await LMApi.PostAPIAsync(message, modelLlama);
                addTextMe(response);

                response = await LMApi.PostAPIAsync(message, model2);
                addTextYou(response);
            }

            if (stop)
            {
                stop = false;
            }
            else
            {
                stop = true;
                //RabbitMQ();
            }
        }
    }
}
