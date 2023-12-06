using System.Linq;
using System.Drawing;

using Pamella;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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
                Text = "Porque é bom separar os dados entre treinamento e testes?",
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
                Text = "Aponte um problema de Aprendizado Supervisionado e Não Supervisionado, respectivamente:",
                Competences = new() {
                    [c6] = 1f,
                    [c7] = 1f
                },
                Alternatives = new() {
                    ["Classificação e Ensemble"] = .2f,
                    ["Regressão e Classificação"] = .5f,
                    ["Clusterização e Classificação"] = 0f,
                    ["Regressão e Clusterização"] = 1f
                }
            },
            q6 = new() {
                Text = "Você deseja criar uma aplicação de ML para descobrir que mês do ano" +
                    " determinados eventos irão ocorrer. Adicionalmente o evento pode ser apontado" +
                    " como só podendo ocorrer em algum momemnto do ano que vem e também pode ser" +
                    " apontado como não vai ocorrer, não se pode saber e vai acontecer em algum mes específico" +
                    " com baixa probabilidade. Quantas classes este problema de classificação possui?",
                Competences = new() {
                    [c8] = 1f
                },
                Alternatives = new() {
                    ["12"] = .2f,
                    ["5"] = .0f,
                    ["16"] = .7f,
                    ["27"] = 1f
                }
            },
            q7 = new() {
                Text = "Para cada imagem a seguir aponte qual algoritmo melhor se correlaciona:",
                Images = new() { Bitmap.FromFile("q7.png") },
                Competences = new() {
                    [c9] = .7f,
                    [c11] = .3f,
                    [c12] = .3f,
                    [c13] = .3f,
                },
                Alternatives = new() {
                    ["SVM; Decision Tree; Clustering; SVM"] = .5f,
                    ["SVM; Decision Tree; KMeans; Regressão Linear"] = 1f,
                    ["Regressão Linear; Decision Tree; KMeans; SVM"] = 0.5f,
                    ["Regressão Linear; Matriz de Confusão; KMeans; Regressão Linear"] = 0f,
                }
            },
            q8 = new() {
                Text = "Você está programando e encontrou o seguinte erro: "+
                    "\"ValueError: could not convert string to float: 'True'.\", o que você entende do erro:",
                Competences = new() {
                    [c9] = .3f,
                    [c10] = 1f,
                    [c11] = .6f
                },
                Alternatives = new() {
                    ["Você não usou um Label Encoder no preprocessamento"] = 1f,
                    ["Você não deveria usar uma Decision Tree para esse problema"] = 0f,
                    ["Você não deveria usar um Naive Bayes para esse problema"] = 0f,
                    ["Você está resolvendo um problema de regressão como se fosse de Classificação"] = .2f
                }
            },
            q9 = new() {
                Text = "Para os próximos três problemas aponte seus nomes: " +
                    "Descobrir se um paciente tem ou não uma doença; " +
                    "Calcular o valor de uma ação ao longo do tempo; " +
                    "Identificar perfil dos usuários em uma plataforma de Streaming.", 
                Competences = new() {
                    [c11] = .4f,
                    [c12] = .4f,
                    [c13] = .4f
                },
                Alternatives = new() {
                    ["Classificação; Classificação; Classificação"] = 0f,
                    ["Clusterização; Regressão; Classificação"] = .5f,
                    ["Classificação; Regressão; Classificação"] = .6f,
                    ["Classificação; Regressão; Clusterização"] = 1f
                }
            },
            q10 = new() {
                Text = "A respeito de Ensemble, aponte o incorreto:", 
                Competences = new() {
                    [c14] = 1f,
                    [c12] = .3f,
                    [c13] = .3f
                },
                Alternatives = new() {
                    ["Boosting é ótimo para regressão ao trabalhar sobre os erros dos modelos anteriores"] = 0f,
                    ["Boosting é altamente paralelizável"] = 1f,
                    ["Bagging é altamente paralelizável"] = .5f,
                    ["Clusterização com Ensemble é possível com uma 'votação' entre os modelos para decidir o cluster de um dado"] = 0f
                }
            }
            ;
        
        this.test = new()
        {
            DataPath = @"S:\COM\Human_Resources\01.Engineering_Tech_School\02.Internal\1 - Meio oficiais\2 - MEIO OFICIAL 2023\Trevisan",
            Competences = new() { 
                c1, c2, c3, c4,
                c5, c6, c7, c8,
                c9, c10, c11, c12,
                c13, c14, c15
            },
            Questions = new() {
                q1, q2, q3, q4, q5,
                q6, q7, q8, q9, q10
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
                g.FillRectangle(5, y, g.Width - 10, jump * (alt.Length / spacing + 1), Brushes.Black);
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - y - 5),
                font, StringAlignment.Near, StringAlignment.Near,
                selected == index ? Brushes.White : Brushes.Black, alt
            );
            y += jump * (alt.Length / spacing + 1);
            index++;
        }
    }

    private void showResult(IGraphics g)
    {
        int y = 5;
        foreach (var comp in test.Competences)
        {
            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Near, StringAlignment.Near,
                comp.Title
            );

            comp.Status = comp.StatusValue switch
            {
                < .4f => CompetenceStatus.Unfit,
                > .4f and < .7f => CompetenceStatus.UnderDevelopment,
                _ => CompetenceStatus.Fit
            };

            g.DrawText(
                new Rectangle(5, y, g.Width - 10, g.Height - 10),
                new Font("Arial", 20), StringAlignment.Far, StringAlignment.Near,
                comp.Status switch {
                    CompetenceStatus.Unfit => Brushes.Red,
                    CompetenceStatus.UnderDevelopment => Brushes.Yellow,
                    CompetenceStatus.Fit => Brushes.Green,
                    _ => Brushes.Black
                },
                comp.Status switch {
                    CompetenceStatus.Unfit => "Inapto",
                    CompetenceStatus.UnderDevelopment => "Em Desenvolvimento",
                    CompetenceStatus.Fit => "Apto",
                    _ => ""
                }
            );
            y += 40;
        }
    }
}