import { Component, OnInit } from '@angular/core';
import { DetalleComponent } from '../../components/detalle/detalle.component';
import { DetallePokemonComponent } from '../../components/detalle-pokemon/detalle-pokemon.component';
import { FotoPokemonComponent } from '../../components/foto-pokemon/foto-pokemon.component';
import { PokemonService } from '../../services/pokemon.service';
import { Resultado } from '../../Interfaces/PokeApi';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [DetalleComponent,DetallePokemonComponent,FotoPokemonComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})


export class HomeComponent implements OnInit{

  constructor(private PokemonService: PokemonService){
  }

  listaPokemon:Resultado[] = [];

  ngOnInit(): void {
    this.CargaLista();
  }

  async CargaLista(){
    this.listaPokemon = [... this.listaPokemon,... await this.PokemonService.GetByPage()];
    console.log(this.listaPokemon);
  }
}
