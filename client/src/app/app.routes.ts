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
        path: 'registerUser',
        loadComponent: () => import('./register-user/register-user').then(m => m.RegisterUser),
        canActivate: [loginGuard]
    },
    {
        path: 'registerArtist',
        loadComponent: () => import('./register-artist/register-artist').then(m => m.RegisterArtist),
        
    }
];
