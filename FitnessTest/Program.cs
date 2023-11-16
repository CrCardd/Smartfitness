using System.Linq;
using System.Drawing;

using Pamella;

App.Open<MainView>();

public class MainView : View
{
    Test test = null;
    int current = 0;
    int selected = 0;
    bool showImage = false;
    int jump = 80;
    int spacing = 60;
    public MainView()
    {
        Competence 
            c1 = new("Conhecer o que é um Dataset"),
            c2 = new("Entender o fluxo básico de Machine Learning"),
            c3 = new("Compreender o fênomeno de Overfitting"),
            c4 = new("Compreender o fênomeno de Underfitting"),
            c5 = new("Compreender a importância da separação dos dados em treinamento e teste"),
            c6 = new("Compreender Aprendizagem Supervisionados"),
            c7 = new("Compreender Aprendizagem Não Supervisionados"),
            c8 = new("Compreender o problema de Classificação"),
            c9 = new("Conhecer os algoritmos básicos"),
            c10 = new("Compreender o processo de Lable Enconder"),
            c11 = new("Compreender os algoritmos de Classificação"),
            c12 = new("Compreender os algoritmos de Regressão"),
            c13 = new("Compreender os algoritmos de Clusterização"),
            c14 = new("Compreender Bagging e Boosting"),
            c15 = new("Compreender Métricas de Avaliação de Performance")
            ;
        
        Question
            q1 = new() {
                Text = "Aponte a alternativa incorreta sobre o conceito de DataSet:",
                Competences = new() {
                    [c1] = .8f
                },
                Alternatives = new() {
                    ["Um csv pode ser usado para armazenar dados de um dataset"] = 0f,
                    ["Um json pode ser usado para armazenar dados de um dataset"] = .3f,
                    ["Um zip com imagens pode ser usado para armazenar dados de um dataset"] = .6f,
                    ["Um dataset precisa estar com todos os dados para ser considerado um dataset"] = 1f
                }
            },
            q2 = new() {
                Text = "Aponte a questão que melhor estrutura o processod e ML:",
                Competences = new() {
                    [c1] = .2f,
                    [c2] = 1f
                },
                Alternatives = new() {
                    ["Análise -> Tratamento -> Treinamento -> Verificação -> Produto"] = .7f,
                    ["Análise -> Tratamento -> Treinamento -> Verificação -> Repita até a satisfação -> Produto"] = 1f,
                    ["Tratamento -> Análise -> Treinamento -> Produto"] = .5f,
                    ["Treinamento -> Análise -> Produto"] = 0f
                }
            },
            q3 = new() {
                Text = "Considere as seguintes matrizes de confusão. Em 4 casos diferentes será mostrado a matriz" +
                    " quando observado dados em relação a predição dos dados de treinamento e teste. Uma linha azul" +
                    " apontará altos valores na matriz de confusão. Para os casos A, B, C e D respectivamente:",
                Images = new() { Bitmap.FromFile("q3.png") },
                Competences = new() {
                    [c3] = .8f,
                    [c4] = .8f,
                    [c15] = .7f
                },
                Alternatives = new() {
                    ["Sucesso; Underfitting; Underfitting; Overfitting"] = .5f,
                    ["Possível Bug; Overfitting; Underfitting; Underfitting"] = 0f,
                    ["Sucesso; Underfitting; Possível Bug; Overfitting"] = 1f,
                    ["Overfitting; Overfitting; Underfitting; Underfitting"] = .2f,
                }
            },
            q4 = new() {
                Text = "Porquê é bom separar os dados entre treinamento e testes?",
                Competences = new() {
                    [c3] = .2f,
                    [c4] = .2f,
                    [c5] = 1f,
                    [c15] = .3f
                },
                Alternatives = new() {
                    ["Pois aumenta a precisão nos dados do treinamento"] = .2f,
                    ["Pois reduz o Overfitting"] = 1f,
                    ["Pois reduz o Underfitting"] = .4f,
                    ["Pois aumenta a precisão nos dados de teste"] = 0f,
                }
            },
            q5 = new() {
                Text = ""
            }
            ;
        
        this.test = new()
        {
            Competences = new() { 
                c1, c2, c3, c4,
                c5, c6, c7, c8,
                c9, c10, c11, c12,
                c13, c14, c15
            },
            Questions = new() {
                q1, q2, q3, q4
            }
        };
    }

    protected override void OnStart(IGraphics g)
    {
        AlwaysInvalidateMode();
        g.SubscribeKeyDownEvent(key => {
            if (key == Input.Escape)
                App.Close();

            if (current >= this.test.Questions.Count)
                return;

            var question = test.Questions[current];
            int altCount = question.Alternatives.Count;
            
            switch (key)
            {   
                case Input.Down:
                    selected++;
                    if (selected >= altCount)
                        selected = 0;
                    break;
                
                case Input.Up:
                    selected--;
                    if (selected < 0)
                        selected = altCount - 1;
                    break;
                
                case Input.I:
                    showImage = !showImage;
                    break;
                
                case Input.W:
                    jump--;
                    break;
                case Input.S:
                    jump++;
                    break;
                
                case Input.Enter:
                    var alt = question
                        .Alternatives.Keys.ToArray()[selected];
                    var correctness = question.Alternatives[alt];

                    foreach (var comp in question.Competences.Keys)
                        comp.StatusValue += correctness
                            * question.Competences[comp];
                    current++;

                    break;
            }
        });
    }

    protected override void OnRender(IGraphics g)
    {
        g.Clear(Color.White);

        if (current >= this.test.Questions.Count)
        {
            showResult(g);
            return;
        }

        var question = this.test.Questions[current];
        var font = new Font("Arial", 40);

        g.DrawText(
            new Rectangle(5, 5, g.Width - 10, g.Height - 10),
            font, StringAlignment.Near, StringAlignment.Near,
            question.Text
        );
        int y = 5 + jump * question.Text.Length / spacing;

        if (question.Images != null && question.Images.Count > 0)
        {
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
                "Pressione i para ver a imagem"
            );
            y += 40;
        }
        
        if (showImage && question.Images != null && question.Images.Count > 0)
        {
            g.DrawImage(
                new RectangleF(5, 5, g.Width - 10, g.Height - 10),
                question.Images[0]
            );
            return;
        }

        int index = 0;
        foreach (var alt in question.Alternatives.Keys)
        {
            if (selected == index)
                g.FillRectangle(5, y, g.Width - 10, jump * alt.Length / spacing, Brushes.Black);
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - y - 5),
                font, StringAlignment.Near, StringAlignment.Near,
                selected == index ? Brushes.White : Brushes.Black, alt
            );
            y += jump * alt.Length / spacing;
            index++;
        }
    }

    private void showResult(IGraphics g)
    {

    }
}