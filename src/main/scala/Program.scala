package tutorial.webapp;

import org.scalajs.dom.document
import org.scalajs.dom.raw.{Element, KeyboardEvent}
import scala.io.Source
import scala.scalajs.js.Dynamic.{ global => g }
import scala.scalajs.js.JSApp
import scala.scalajs.js.timers._


object TutorialApp extends JSApp {

    var lastKeyString = ""

    val tickMs = 50

    def main(): Unit = {
        val level = WholeMap.levelOne
        val player = new Player(10, 10, ">", true)
        level(player.posX)(player.posY) = 1
        val domMap = new DomMap(new Renderer())
        domMap.render(level, 0, 80, player)

        document.addEventListener("keyup", {
            (event: KeyboardEvent) => {
                lastKeyString = event.key
            }
        }, false)

        setTimeout(tickMs) {
            doTick(0, player, level, domMap, 0)
        }
    }

    def doTick(tickNum: Int, player: Player, level: Array[Array[Int]], domMap: DomMap, currentCol: Int): Unit = {

        if (tickNum % 6 == 0) {
            val (newX, newY) = lastKeyString match {
                case "ArrowUp"    => (player.posX - 1, player.posY + 1)
                case "ArrowDown"  => (player.posX + 1, player.posY + 1)
                case "ArrowLeft"  => (player.posX, player.posY)
                case "ArrowRight" => (player.posX, player.posY + 2)
                case " "          => // fire laser
                    level(player.posX)(player.posY + 2) = 3
                    (player.posX, player.posY + 1)
                case _            => (player.posX, player.posY + 1)
            }
            lastKeyString = ""

            val newPlayer = 
                if (newX > 0 && newX < level.length && newY > 0 && newY < level(0).length) {
                    if (level(newX)(newY) != 0) {
                        player.meurt()
                    } else {
                        // erase last player location, put in next
                        level(player.posX)(player.posY) = 0
                        level(newX)(newY) = 1
                        player.deplacer(newX, newY)
                    }
                } else {
                    player
                }

            domMap.render(level, currentCol, currentCol + 80, newPlayer)

            if (currentCol < level(0).length) {
                setTimeout(tickMs) {
                    doTick(tickNum + 1, newPlayer, level, domMap, currentCol + 1)
                }
            }

        } else { // do lasers

            for (col <- (level(0).length - 2) until 0 by -1) {
                for (row <- 0 until level.length) {
                    if (level(row)(col) == 3) { // laser
                        if (level(row)(col + 1) == 2) { // collision
                            level(row)(col + 1) = 4
                            level(row)(col) = 0
                        } else {
                            level(row)(col + 1) = 3
                            level(row)(col) = 0
                        }
                    }
                }
            }
            if (currentCol < level(0).length) {
                setTimeout(tickMs) {
                    doTick(tickNum + 1, player, level, domMap, currentCol)
                }
            }
        }
    }
}

case class Player(posX: Int, posY: Int, sprite: String, vivant: Boolean) {
    def deplacer(x: Int, y: Int) = this.copy(posX = x, posY = y)
    def meurt() = this.copy(sprite = "x", vivant = false)
}

object WholeMap {
    val dataLiteral = """
44444422222244422222222224444444444444444444422222222222444444444444444444444444444444444444444444444444444444444444444444444
00000442222440042222222240000000000000000004442222222224422224222222222244444422000000000000000000000000000000000000000000000
00000044224400004222222400000000000000000000444222222244222222422222244444222220000000200000000000000000000000000000000000000
00000044240000004422224000000000000000000000044422222442222222242222444442222200000002420000000000000000000000000000000000000
00000444400000000442244000000000000000000000004442224422222222242224444422222000000002222000000000000000000000000000000000000
00004440000000000042240000000000000000000000000442244222002222224444422222200000000002244200000000000000000000000000000000000
00044000000000000044000000000000000000000000000042442200000022220044222222000000000022444420000000000000000000000000000000000
00040000000000000440004440000000000000000000000044400000000002240002222220000000002224444442000000000000000000000000000000000
00000000000000000000044224440000000000000000000444000000000000044000222200000000022244444444200000000000000000000000000000000
00000000000000000000422222244000000000000000004400000000000000042400022000000002222244444444420000000000000000000000000000000
00000000000000000004422222240000000000000000004000000000000000442240000000000022224444444444442000000000000000000000000000000
00000000000000000000442222400000000000000000000000000000000004422224000000000222444444444444444200000000000000000000000000000
00000400000000000000044224000000000000000000000000000000000044222222400000002224444444444444444422000000000000000000000000000
00000440000000000000004240002200000000000000000000000000000004222222440000022244444444444444444422200000000000000000000000000
00000044400000000000004400022220000000000000000000000000000000442244400000222244444444444444444442220000000000000000000000000
00000004440000000000224000222222000000000000400000000000004400044400000002222444444444444444444444222000000000000000000000000
00000004244220000002222004422222200000000000440000000000000440000000000022244444444444444444444444422000000000000000000000000
00000044224422200222222444442222220000000000044400000000000422400000000022244444444444444444444444220000000000000000000000000
00000444222442222222224222444442222200000000004444000000004422440000000022244444444444444444444444222000000000000000000000000
00004442222244222222224222244444222220000000000442400000044222240000000022244444444444444444444444222000000000000000000000000
00044422222224422222242222224444422222000000000442244000042222224000000222444444444444444444444444442200000000000000000000000
00444222222222442222422222222224444442200000004422224400422222222400022222444444444444444444444444442220000000000000000000000
44442222222222244444444444444444444444444444444222222444222222222244444444444444444444444444444444444444444444444444444444444
"""
    val levelOne: Array[Array[Int]] = dataLiteral.trim().split("\n").map{ line => 
        line.trim().toArray.map(_.toString.toInt)
    }
}

class DomMap(renderer: Renderer) {

    def render(data: Array[Array[Int]], startCol: Int, endCol: Int, player: Player): Unit = {

        val t = document.querySelector("table")
        if (t != null) document.body.removeChild(t)

        val table = document.createElement("table")
        document.body.appendChild(table)

        for (r <- 0 until data.length) {
            val tr = document.createElement("tr")
            table.appendChild(tr)

            for (c <- startCol until Math.min(endCol, data(0).length)) {
                val td = document.createElement("td")
                tr.appendChild(td)
                // td.textContent = data(r)(c).toString
                renderer.render(data(r)(c), td, player)
            }
        }
    }
}

class Renderer {
    def render(num: Int, el: Element, player: Player): Unit = {
        num match {
            case 0 =>
                el.setAttribute("style", "")
                el.textContent = " "
            case 1 =>
                el.setAttribute("style", "color: #0ff")
                el.textContent = player.sprite
            case 2 =>
                el.setAttribute("style", "color: #999")
                el.textContent = "█"
            case 3 =>
                el.setAttribute("style", "color: #f00")
                el.textContent = "■"
            case 4 =>
                el.setAttribute("style", "color: #0f0")
                el.textContent = "█"
        }
    }
}
