import { Routes } from '@angular/router';
import { Music } from './music/music';
import { loginGuard } from './components/guards/login.guards';

export const routes: Routes = [
    {
        path: 'music',
        component:Music
    },
    {
        path: '**',
        redirectTo: 'music',
        pathMatch: 'full'
    },
    {
        path: 'register',
        loadComponent: () => import('./register/register').then(m => m.Register),
        canActivate: [loginGuard]
    },
    {
        path: 'login',
        loadComponent: () => import('./login/login').then(m => m.Login),
        canActivate: [loginGuard]
    },
];
