import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { UserDto } from '../models/user.model';
import { FormsModule } from '@angular/forms'; 
import { AuthService } from '../services/auth.service'; // Asegúrate de importar el servicio
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule], 
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  user: UserDto = { email: '', password: '' };
  serverResponse: string = ''; // Para almacenar la respuesta del servidor

  constructor(private authService: AuthService) {}

  login() {
    this.serverResponse = ''; // Limpiar el mensaje antes de iniciar sesión
    this.authService.login(this.user).subscribe({
      next: (response) => {
        console.log('Usuario autenticado:', response);
        this.serverResponse = 'Inicio de sesión exitoso.'; // Mensaje de éxito
        // Aquí puedes almacenar el token o redirigir al usuario
      },
      error: (error) => {
        console.error('Error al iniciar sesión:', error);
        this.serverResponse = 'Error al iniciar sesión: ' + error.error?.message || 'Ha ocurrido un error.';
      }
    });
  }
}
