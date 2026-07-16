import { MediaItem } from './common.model';

export const POSITION_OPTIONS = [
  'Closed Guard',
  'Half Guard',
  'Side Control',
  'Mount',
  'Back Control',
  'Standing'
];

export interface TechniqueStep {
  id: string;
  order: number;
  description: string;
}

export interface Technique {
  id: string;
  name: string;
  position: string;
  description: string;
  steps: TechniqueStep[];
  media: MediaItem[];
}

export interface TechniqueInput {
  name: string;
  position: string;
  description: string;
  steps: string[];
}
