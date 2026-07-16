import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Technique } from '../../core/models/technique.model';
import { TechniqueService } from './technique.service';

@Component({
  selector: 'app-technique-list',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatCardModule, MatIconModule],
  templateUrl: './technique-list.component.html'
})
export class TechniqueListComponent implements OnInit {
  private readonly techniqueService = inject(TechniqueService);

  readonly techniques = signal<Technique[]>([]);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.techniqueService.getAll().subscribe((techniques) => this.techniques.set(techniques));
  }

  remove(id: string): void {
    this.techniqueService.delete(id).subscribe(() => this.load());
  }
}
