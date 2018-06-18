module Client.ContactsView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open ClientModelMsg
open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.React

module ChartsPG =
    open Fable.Recharts   
    open Fable.Recharts.Props
    module R = Fable.Helpers.React
    module P = R.Props

    let margin t r b l =
        Chart.Margin { top = t; bottom = b; right = r; left = l }

    type [<Pojo>] Data =
        { name: string; uv: int; pv: int; amt: int }
    let data =
        [| { name = "Page A"; uv = 4000; pv = 2400; amt = 2400 }
           { name = "Page B"; uv = 3000; pv = 1398; amt = 2210 }
           { name = "Page C"; uv = 2000; pv = 9800; amt = 2290 }
           { name = "Page D"; uv = 2780; pv = 3908; amt = 2000 }
           { name = "Page E"; uv = 1890; pv = 4800; amt = 2181 }
           { name = "Page F"; uv = 2390; pv = 3800; amt = 2500 }
           { name = "Page G"; uv = 3490; pv = 4300; amt = 2100 }
        |]

    let lineChartSample() =
        lineChart
            [ margin 5. 20. 5. 0.
              Chart.Width 600.
              Chart.Height 300.
              Chart.Data data ]
            [ line
                [ Cartesian.Type Monotone
                  Cartesian.DataKey "uv"
                  P.Stroke "#8884d8"
                  P.StrokeWidth 2. ]
                []
              cartesianGrid
                [ P.Stroke "#ccc"
                  P.StrokeDasharray "5 5" ]
                []
              xaxis [Cartesian.DataKey "name"] []
              yaxis [] []
              tooltip [] []
            ]

    type PieData = {
        name: string
        value: int
    }
    let pieData = [ {name = "Group A"; value = 400} 
                    {name = "Group B"; value = 300}
                    {name = "Group C"; value = 300}
                    {name = "Group D"; value = 200} ];
    let COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042"];

    let RADIAN = System.Math.PI / 180.;      

    let radialChartSample() =
        pieChart
            [ margin 5. 20. 5. 0.
              Chart.Width 600.
              Chart.Height 300. ]
            [ pie
                [   Chart.Data data
                    Chart.Cx 420.
                    Chart.Cy 200.
                    Chart.StartAngle 180.
                    Chart.EndAngle 0.
                    Chart.InnerRadius 60.
                    Chart.OuterRadius 80.
                    // Custom ("fill", "#8884d8")
                    Polar.PaddingAngle 5. ]
                [
                    cell [ !!(Cell.Fill "#8884d8") ] []
                ]
            ]
    

type [<Pojo>] GaugeChartProps = { width: int }
let GaugeChart : GaugeChartProps -> ReactElement = importDefault "../GaugeChart.jsx"
// let GaugeChart : unit -> ReactElement = import "GaugeChart" "../GaugeChart.jsx"


let view  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [   str "Contacts" 
            ChartsPG.lineChartSample()
            // ChartsPG.radialChartSample()
            ofFunction GaugeChart { width = 500 } [ p[] [ str "asasdasdasdasd"]]
            GaugeChart { width = 500 }
        ]