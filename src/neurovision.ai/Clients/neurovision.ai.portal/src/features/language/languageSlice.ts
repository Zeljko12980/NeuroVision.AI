import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface LanguageState {
  current: string;
}

const LOCAL_STORAGE_KEY = 'appLanguage';

const initialLanguage = localStorage.getItem(LOCAL_STORAGE_KEY) || 'en';

const initialState: LanguageState = {
  current: initialLanguage,
};

const languageSlice = createSlice({
  name: 'language',
  initialState,
  reducers: {
    setLanguage: (state, action: PayloadAction<string>) => {
      state.current = action.payload;
      localStorage.setItem(LOCAL_STORAGE_KEY, action.payload);
    },
  },
});

export const { setLanguage } = languageSlice.actions;
export default languageSlice.reducer;