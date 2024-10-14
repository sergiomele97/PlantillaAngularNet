import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet], // Importa solo RouterOutlet
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'] // Asegúrate de que sea "styleUrls"
})
export class AppComponent {
  title = 'plantilla';
}
