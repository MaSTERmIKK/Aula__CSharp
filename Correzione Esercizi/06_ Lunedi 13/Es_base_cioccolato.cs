// Classe base
class Cioccolato {
    String tipo;
    int percentualeCacao;

    public Cioccolato(String tipo, int percentualeCacao) {
        this.tipo = tipo;
        this.percentualeCacao = percentualeCacao;
    }

    public void produce() {
        System.out.println("Produco cioccolato " + tipo + " con " + percentualeCacao + "% di cacao.");
    }
}

// Sottoclasse 1: Cioccolato Caldo
class CioccolatoCaldo extends Cioccolato {
    double temperatura;

    public CioccolatoCaldo(String tipo, int percentualeCacao, double temperatura) {
        super(tipo, percentualeCacao);
        this.temperatura = temperatura;
    }

    @Override
    public void produce() {
        System.out.println("Produco cioccolato caldo " + tipo + " servito a " + temperatura + "°C.");
    }
}

// Sottoclasse 2: Tavoletta
class Tavoletta extends Cioccolato {
    int grammi;

    public Tavoletta(String tipo, int percentualeCacao, int grammi) {
        super(tipo, percentualeCacao);
        this.grammi = grammi;
    }

    @Override
    public void produce() {
        System.out.println("Produco tavoletta di cioccolato " + tipo + " da " + grammi + " grammi.");
    }
}

// Classe principale
public class Fabbrica {
    public static void main(String[] args) {
        // Polimorfismo: una lista di oggetti diversi trattati come Cioccolato
        Cioccolato c1 = new Cioccolato("fondente", 70);
        Cioccolato c2 = new CioccolatoCaldo("al latte", 50, 45.5);
        Cioccolato c3 = new Tavoletta("bianco", 30, 100);

        // Chiamata dei metodi in modo polimorfico
        Cioccolato[] prodotti = { c1, c2, c3 };
        for (Cioccolato c : prodotti) {
            c.produce();
        }
    }
}
