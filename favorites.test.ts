import { Book, FavoritesManager } from './index';

// Mock window.confirm for testing
const mockConfirm = jest.fn();
Object.defineProperty(window, 'confirm', { value: mockConfirm });

describe('FavoritesManager', () => {
    let favoritesManager: FavoritesManager;
    let sampleBook: Book;

    beforeEach(() => {
        favoritesManager = new FavoritesManager();
        sampleBook = {
            id: '1',
            title: 'Test Book',
            author: 'Test Author',
            isbn: '978-0-123456-78-9',
            publishedYear: 2023
        };
        mockConfirm.mockClear();
    });

    describe('addToFavorites', () => {
        test('should add a book to favorites successfully', () => {
            const result = favoritesManager.addToFavorites(sampleBook);
            
            expect(result).toBe(true);
            expect(favoritesManager.getFavoritesCount()).toBe(1);
            expect(favoritesManager.isFavorite('1')).toBe(true);
        });

        test('should not add duplicate books to favorites', () => {
            favoritesManager.addToFavorites(sampleBook);
            const result = favoritesManager.addToFavorites(sampleBook);
            
            expect(result).toBe(false);
            expect(favoritesManager.getFavoritesCount()).toBe(1);
        });
    });

    describe('removeFromFavorites', () => {
        beforeEach(() => {
            favoritesManager.addToFavorites(sampleBook);
        });

        test('should remove a book from favorites with confirmation', async () => {
            mockConfirm.mockReturnValue(true);
            
            const result = await favoritesManager.removeFromFavorites('1', true);
            
            expect(result).toBe(true);
            expect(favoritesManager.getFavoritesCount()).toBe(0);
            expect(favoritesManager.isFavorite('1')).toBe(false);
            expect(mockConfirm).toHaveBeenCalledWith(
                'Are you sure you want to remove "Test Book" by Test Author from your favorites?'
            );
        });

        test('should cancel removal when user declines confirmation', async () => {
            mockConfirm.mockReturnValue(false);
            
            const result = await favoritesManager.removeFromFavorites('1', true);
            
            expect(result).toBe(false);
            expect(favoritesManager.getFavoritesCount()).toBe(1);
            expect(favoritesManager.isFavorite('1')).toBe(true);
        });

        test('should remove a book without confirmation when showConfirmation is false', async () => {
            const result = await favoritesManager.removeFromFavorites('1', false);
            
            expect(result).toBe(true);
            expect(favoritesManager.getFavoritesCount()).toBe(0);
            expect(favoritesManager.isFavorite('1')).toBe(false);
            expect(mockConfirm).not.toHaveBeenCalled();
        });

        test('should return false when trying to remove non-existent book', async () => {
            const result = await favoritesManager.removeFromFavorites('non-existent', false);
            
            expect(result).toBe(false);
            expect(favoritesManager.getFavoritesCount()).toBe(1);
        });
    });

    describe('getFavorites', () => {
        test('should return a copy of favorites array', () => {
            favoritesManager.addToFavorites(sampleBook);
            const favorites = favoritesManager.getFavorites();
            
            expect(favorites).toHaveLength(1);
            expect(favorites[0]).toEqual(sampleBook);
            
            // Modifying returned array should not affect internal favorites
            favorites.push({
                id: '2',
                title: 'Another Book',
                author: 'Another Author'
            });
            
            expect(favoritesManager.getFavoritesCount()).toBe(1);
        });

        test('should return empty array when no favorites', () => {
            const favorites = favoritesManager.getFavorites();
            expect(favorites).toEqual([]);
        });
    });

    describe('isFavorite', () => {
        test('should return true for favorite books', () => {
            favoritesManager.addToFavorites(sampleBook);
            expect(favoritesManager.isFavorite('1')).toBe(true);
        });

        test('should return false for non-favorite books', () => {
            expect(favoritesManager.isFavorite('1')).toBe(false);
        });
    });

    describe('getFavoritesCount', () => {
        test('should return correct count of favorites', () => {
            expect(favoritesManager.getFavoritesCount()).toBe(0);
            
            favoritesManager.addToFavorites(sampleBook);
            expect(favoritesManager.getFavoritesCount()).toBe(1);
            
            favoritesManager.addToFavorites({
                id: '2',
                title: 'Second Book',
                author: 'Second Author'
            });
            expect(favoritesManager.getFavoritesCount()).toBe(2);
        });
    });

    describe('clearFavorites', () => {
        test('should clear all favorites', () => {
            favoritesManager.addToFavorites(sampleBook);
            favoritesManager.addToFavorites({
                id: '2',
                title: 'Second Book',
                author: 'Second Author'
            });
            
            expect(favoritesManager.getFavoritesCount()).toBe(2);
            
            favoritesManager.clearFavorites();
            
            expect(favoritesManager.getFavoritesCount()).toBe(0);
            expect(favoritesManager.getFavorites()).toEqual([]);
        });
    });
});