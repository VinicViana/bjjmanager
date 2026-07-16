import { MediaItem } from './common.model';

export type SelfEvaluation = 'VeryBad' | 'BelowExpectations' | 'Average' | 'Good' | 'Excellent';

export const SELF_EVALUATION_OPTIONS: { value: SelfEvaluation; label: string }[] = [
  { value: 'VeryBad', label: '1 - Very bad training' },
  { value: 'BelowExpectations', label: '2 - Below expectations' },
  { value: 'Average', label: '3 - Average' },
  { value: 'Good', label: '4 - Good' },
  { value: 'Excellent', label: '5 - Excellent' }
];

export interface TrainingSession {
  id: string;
  trainingDate: string;
  academyName: string;
  notes: string | null;
  selfEvaluation: SelfEvaluation;
  createdAtUtc: string;
  media: MediaItem[];
}

export interface TrainingSessionInput {
  trainingDate: string;
  academyName: string;
  selfEvaluation: SelfEvaluation;
  notes: string | null;
}

export interface TrainingFilter {
  date?: string;
  month?: number;
  year?: number;
}

export interface DailyNote {
  date: string;
  averageScore: number;
  sessionCount: number;
}

export interface TrainingNotesSummary {
  dailyNotes: DailyNote[];
  overallAverage: number;
  totalSessions: number;
}
