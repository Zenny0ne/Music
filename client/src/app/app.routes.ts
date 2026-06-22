import { Routes } from '@angular/router';
import { Music } from './music/music';
import { Register } from './register/register';
import { Login } from './login/login';

export const routes: Routes = [
    {
        path: 'music',
        component:Music
    },
    {
        path: 'register',
        component: Register
    },
    {
        path: 'login',
        component: Login
    },
    {
        path: '**',
        redirectTo: 'music',
        pathMatch: 'full'
    },
];
