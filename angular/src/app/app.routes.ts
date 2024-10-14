import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component'; // Asegúrate de que la ruta sea correcta
import { RegistroComponent } from './registro/registro.component'; // Asegúrate de que la ruta sea correcta
import { WelcomeComponent } from './welcome/welcome.component'; // Asegúrate de que la ruta sea correcta

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // Redirige a login si la ruta es vacía
  { path: 'login', component: LoginComponent }, // Ruta para el login
  { path: 'registro', component: RegistroComponent }, // Ruta para el registro
  { path: 'welcome', component: WelcomeComponent }, // Ruta para la pantalla de bienvenida
];
