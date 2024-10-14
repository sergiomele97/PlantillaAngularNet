// src/app/registro/registro.component.ts
import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserDto } from '../models/user.model';
import { FormsModule } from '@angular/forms'; // Importa FormsModule
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './registro.component.html',
  styleUrls: ['./registro.component.css']
})
export class RegistroComponent {
  user: UserDto = { email: '', password: '' }; // Inicializa el modelo
  serverResponse: string = ''; // Para almacenar la respuesta del servidor

  constructor(private authService: AuthService) {} // Inyecta el servicio

  register() {
    this.serverResponse = ''; // Limpiar el mensaje antes de registrar
    this.authService.register(this.user).subscribe({
      next: (response) => {
        console.log('Usuario registrado:', response);
        this.serverResponse = 'Registro exitoso. Puedes iniciar sesión ahora.'; // Mensaje de éxito
      },
      error: (error) => {
        console.error('Error al registrar:', error);
        // Asegúrate de que el error sea accesible y manejable
        if (error.error && error.error.Message) {
            this.serverResponse = 'Error al registrar: ' + error.error.Message; // Mensaje de error
        } else {
            this.serverResponse = 'Error desconocido. Por favor, intenta de nuevo.'; // Mensaje de error por defecto
        }
      }
    });
}
}
